using Corsa.Domain.Models.Requests;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Corsa.Domain.Processing.Moduls;
using System.Linq;
using System.Collections.Concurrent;
using Corsa.Domain.Models.Schedule;
using System.Collections;

namespace Corsa.Infrastructure.Tasks
{   
    public abstract class ScheduleServiceBase : TimedHostedService
    {
        protected ScheduleServiceBase(IServiceProvider provider, IScheduleTaskQueue taskQueue, int period) : base(provider, period)
        {
            TaskQueue = taskQueue;
        }

        protected IScheduleTaskQueue TaskQueue { get; set; }
    }

    public class ScheduleBuilderService : ScheduleServiceBase
    {
        public int Range { get; set; } = 600000;

        public ScheduleBuilderService(IServiceProvider provider, IScheduleTaskQueue taskQueue ) : base(provider, taskQueue, 15000)
        {            
            
        }

        public override void Run()
        {
            SyncQueueToRepository();

            CleanUpQueue();

            SyncRepositoryToQueue();

            PopulateTaskScheduleQueue();
        }

        private void SyncQueueToRepository()
        {
            var repository = Provider.GetService<ISourceRepository>();

            foreach (var task in TaskQueue)
            {
                var targetTask = repository.GetScheduleTask(task.Id);
                if (targetTask != null)
                {
                    repository.UpdateScheduleTask(task);                    
                }
                else
                {
                    repository.AddScheduleTask(task);
                }
            }
        }

        private void SyncRepositoryToQueue()
        {
            var repository = Provider.GetService<ISourceRepository>();
            var registry = Provider.GetService<IProjectModuleRegistry>();

            var currentDateTime = DateTime.Now;
            var actualTasks = repository.ScheduleTasks().Where(task =>
            (task.State == ScheduleTaskState.Initiated && task.DateTime <= task.DateTime.AddMilliseconds(Range)) ||
                Math.Abs((currentDateTime - task.DateTime).TotalMilliseconds) <= Range);

            foreach (var task in actualTasks)
            {
                var targetTask = TaskQueue.Find(item => item.Id == task.Id);
                if (targetTask == null)
                {
                    var moduleTask = registry.CreateModuleTask(task.Module.Code, new object[] { Provider, task.ProjectModuleId, task.Module.Project.UserId });
                    task.Task = moduleTask;

                    TaskQueue.Add(task);
                }
            }
        }

        private void CleanUpQueue()
        {
            var currentDateTime = DateTime.Now;
            TaskQueue.Remove(task => Math.Abs((currentDateTime - task.DateTime).TotalMilliseconds) > Range);
        }

        public void PopulateTaskScheduleQueue()
        {
            var repository = Provider.GetService<ISourceRepository>();
            var projectModules = repository.GetProjectModules();
            var moduleRegistry = Provider.GetService<IProjectModuleRegistry>();
            var registry = Provider.GetService<IProjectModuleRegistry>();

            foreach (var module in projectModules)
            {
                var schedules = new List<TaskSchedule>();
                schedules.AddRange(module.DailySchedules);
                schedules.AddRange(module.OneTimeSchedules);

                var userSettings = repository.GetUserSettings(module.Project.UserId);

                foreach (var item in schedules)
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(userSettings.TimeZoneId);

                    var clientDateTime = timeZone==null? DateTime.Now: TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);

                    var targetTime = item.AlarmTime(clientDateTime);

                    var diference = targetTime - DateTime.Now;

                    if (Math.Abs(diference.TotalMilliseconds) <= Range)
                    {
                        var targetModuleTask = this.TaskQueue.Find(task => task.Task.Id == module.Id && task.DateTime == targetTime);
                        if (targetModuleTask == null)
                        {
                            var moduleTask = registry.CreateModuleTask(module.Code, new object[] { Provider, module.Id, module.Project.UserId });
                            this.TaskQueue.Add(new ScheduleTask(module.Id, targetTime, moduleTask));
                        }
                    }
                }
            }
        }
    }
    
    public class RuntimeTaskService
    {
        ITaskPipline TaskPipline { get; set; }

        object _lock = new object();

        public RuntimeTaskService(ITaskPipline taskPipline)
        {
            TaskPipline = taskPipline;           
            TaskPipline.TaskCompleted += TaskPipline_TaskCompleted;
            TaskPipline.TaskStarted += TaskPipline_TaskStarted;
            TaskPipline.TaskAdded += TaskPipline_TaskAdded;
        }

        ConcurrentDictionary<int, ITask> TaskStates { get; set; } = new ConcurrentDictionary<int, ITask>();
        public ITask this[int moduleId]
        {
            get
            {
                ITask task;
                TaskStates.TryGetValue(moduleId, out task);
                return task;
            }
        }

        public bool Add(ITask task)
        {
            lock (_lock)
            {
                if (!TaskStates.ContainsKey(task.Id))
                {
                    TaskPipline.Add(task);

                    return true;
                }
            }

            return false;
        }

        public void Run()
        {
            if (!TaskPipline.IsRunned)
            {
                Task.Run(() =>
                {
                    TaskPipline.Run();
                });
            }
        }

        public bool EraseState(int index)
        {
            ITask targetTask;
            if (TaskStates.TryGetValue(index, out targetTask))
            {
                if (targetTask.State.State == TaskState.Completed || targetTask.State.State == TaskState.CompletedWithError)
                {
                    return TaskStates.TryRemove(index, out targetTask);
                }
            }
            return false;
        }

        private void TaskPipline_TaskStarted(object sender, TaskEventArgs e)
        {

        }

        private void TaskPipline_TaskCompleted(object sender, TaskEventArgs e)
        {

        }

        public bool Remove(int id)
        {
            return false;
        }

        public bool Drop(int index)
        {
            throw new System.NotImplementedException();
        }

        private void TaskPipline_TaskAdded(object sender, TaskEventArgs e)
        {
            EraseState(e.Task.Id);
            TaskStates.TryAdd(e.Task.Id, e.Task);
        }
    }

    public class TaskPiplineService : TimedHostedService
    {
        RuntimeTaskService RuntimeTaskService  { get; set; }
        
        public TaskPiplineService(IServiceProvider provider, RuntimeTaskService runtimeTaskService) : base(provider, 5000)
        {
            this.RuntimeTaskService = runtimeTaskService;
        }

        public override void Run()
        {
            RuntimeTaskService.Run();
        }        
    }

    public class ScheduleService : ScheduleServiceBase
    {
        ITaskPipline Pipline { get; set; }

        public ScheduleService(IServiceProvider provider, ITaskPipline pipline, IScheduleTaskQueue schedule) : base(provider, schedule, 15000)
        {
            this.Pipline = pipline;
        }

        public override void Run()
        {            
            var schedules = TaskQueue.GetTasks(task => task.State==ScheduleTaskState.Initiated && task.DateTime <= DateTime.Now);

            foreach (var schedule in schedules)
            {
                Pipline.Add(schedule.Task);
                schedule.State = ScheduleTaskState.Copmleted;
            }
        }
    }

    public class ScheduleTaskQueue : IScheduleTaskQueue
    {
        List<ScheduleTask> _tasks = new List<ScheduleTask>();
        
        public void Add(ScheduleTask task)
        {
            _tasks.Add(task);
        }

        public void Remove(ScheduleTask task)
        {
            _tasks.Remove(task);
        }

        public void Remove(Predicate<ScheduleTask> filter)
        {
            _tasks.RemoveAll(filter);
        }

        public ScheduleTask Find(Predicate<ScheduleTask> filter)
        {
            return _tasks.FirstOrDefault(task => filter(task));
        }

        public IEnumerator<ScheduleTask> GetEnumerator()
        {
            return _tasks.GetEnumerator();
        }

        public ICollection<ScheduleTask> GetTasks(Predicate<ScheduleTask> filter)
        {
            return _tasks.Where(task => filter(task)).ToList() ;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Corsa.Domain.Tasks
{
    public class TaskPipline : ITaskPipline
    {
        public event EventHandler<TaskEventArgs> TaskStarted;

        public event EventHandler<TaskEventArgs> TaskCompleted;

        public event EventHandler<TaskEventArgs> TaskAdded;

        public event EventHandler<EventArgs> Stoped;

        public event EventHandler<EventArgs> Runned;
    
        object _lock = new object();
        Stopwatch PiplineTimeWather = new Stopwatch();
        ConcurrentQueue<ITask> Tasks { get; } = new ConcurrentQueue<ITask>();
        CancellationTokenSource Shutdown = new CancellationTokenSource();

        public bool IsRunned{get; set;}
        
        public bool Add(ITask task)
        {
            Tasks.Enqueue(task);

            if (TaskAdded != null)
            {
                TaskAdded(this, new TaskEventArgs(task));
            }

            return true;
        }

        public bool Run()
        {            
            PiplineTimeWather.Start();
            Shutdown = new CancellationTokenSource();

            if (Runned != null)
            {
                Runned(this, new EventArgs());
            }
            
            IsRunned = true;

            if (Tasks.Count > 0)
            {
                while (!Shutdown.IsCancellationRequested && Tasks.Count > 0)
                {
                    ITask task;
                    if (Tasks.TryDequeue(out task))
                    {
                        RunTask(task);
                    }
                }
            }
            
            PiplineTimeWather.Stop();

            IsRunned = false;

            if (Stoped != null)
            {
                Stoped(this, new EventArgs());
            }

            return true;
        }

        public void RunTask(ITask task)
        {
            Task.Run(() =>
            {
                if (TaskStarted != null)
                {
                    TaskStarted(this, new TaskEventArgs(task));
                }

                task.Start();

                if (TaskCompleted != null)
                {
                    TaskCompleted(this, new TaskEventArgs(task));
                }
            });
        }

        public bool Stop()
        {
            Shutdown.Cancel();
            return true;
        }
    }
}

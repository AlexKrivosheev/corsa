using Corsa.Domain.Models.Schedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Tasks
{
    public interface IScheduleTaskQueue: IEnumerable<ScheduleTask>
    {
        void Add(ScheduleTask task);

        void Remove(ScheduleTask task);

        void Remove(Predicate<ScheduleTask> filter);
        
        ScheduleTask Find(Predicate<ScheduleTask> filter);

        ICollection<ScheduleTask> GetTasks(Predicate<ScheduleTask> filter);


    }
}

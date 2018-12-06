using Corsa.Domain.Models.Projects;
using Corsa.Domain.Tasks;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.Schedule
{
    public class ScheduleTask
    {
        public ScheduleTask()
        {            
        }

        public ScheduleTask(int projectModuleId, DateTime dateTime, ITask task)
        {
            ProjectModuleId = projectModuleId;
            DateTime = dateTime;
            Task = task;
        }

        public int Id { get; set; }

        public int ProjectModuleId { get; set; }

        public DateTime DateTime { get; set; }

        public ScheduleTaskState State { get; set; }

        [ForeignKey("ProjectModuleId")]
        public ProjectModule Module { get; set; }

        [NotMapped]
        public ITask Task { get; set; }
    }

    public enum ScheduleTaskState
    {
        Initiated,
        Copmleted
    }


}

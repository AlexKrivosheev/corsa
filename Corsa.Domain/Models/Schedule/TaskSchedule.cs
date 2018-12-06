using Corsa.Domain.Models.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Corsa.Domain.Tasks
{
    public enum ScheduleType
    {
        OnceTime,
        Weekly
    }

    public abstract class TaskSchedule
    {
        public int Id { get; set; }

        public int ProjectModuleId { get; set; }

        public string Description { get; set; }

        [ForeignKey("ProjectModuleId")]
        public ProjectModule ProjectModul { get; set; } = new ProjectModule();

        public abstract DateTime AlarmTime(DateTime dateTime);
    }
}

using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.Projects
{
    [Table("ProjectModules")]
    public class ProjectModule
    { 
        public  int Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public int ProjectId { get; set; }

        public DateTime CreatedTime { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [ForeignKey("ProjectModuleId")]
        public List<LinkedModule> LinkedModules { get; set; } = new List<LinkedModule>();

        [ForeignKey("ProjectModuleId")]
        public List<DailySchedule> DailySchedules { get; set; } = new List<DailySchedule>();

        [ForeignKey("ProjectModuleId")]
        public List<OneTimeSchedule> OneTimeSchedules { get; set; } = new List<OneTimeSchedule>();
    }
}

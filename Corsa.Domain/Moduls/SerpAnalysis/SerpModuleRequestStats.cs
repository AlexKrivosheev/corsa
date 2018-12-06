using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.SearchEngines;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace Corsa.Domain.Moduls.SerpAnalysis
{
    [Table("SerpModulePositions")]
    public class SerpModuleRequestStats
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ProjectModuleResultId { get; set; }
        public int SerpModuleRequestId { get; set; }

        public string Details { get; set; }

        public DateTime DetectionTime { get; set; } = new DateTime(2000, 01, 01);

        [ForeignKey("ProjectModuleResultId")]
        public ProjectModuleResult ModuleResult { get; set; }

        [ForeignKey("SerpModuleRequestId")]
        public SerpModuleRequest Request { get; set; }
    }
}

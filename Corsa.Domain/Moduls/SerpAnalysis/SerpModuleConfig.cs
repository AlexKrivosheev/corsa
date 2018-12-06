using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Moduls.SerpAnalysis
{
   public class SerpModuleConfig : ModuleСonfig
    {
        public string Url { get; set; }

        public int? DataProviderId { get; set; }

        public int RequestAttempt { get; set; }

        [ForeignKey("DataProviderId")]
        public ProjectModule DataProvider { get; set; } 

        [ForeignKey("SerpModuleConfigId")]
        public List<SerpModuleRequest> Requests { get; set; } = new List<SerpModuleRequest>();

        [NotMapped]
        public List<DailySchedule> DailySchedules
        {
            get
            {
                return this.ProjectModule.DailySchedules;
            }
            set
            {
                this.ProjectModule.DailySchedules = value;
            }
        }

        [NotMapped]
        public List<OneTimeSchedule> OneTimeSchedules
        {
            get
            {
                return this.ProjectModule.OneTimeSchedules;
            }
            set
            {
                this.ProjectModule.OneTimeSchedules = value;
            }
        }


        [NotMapped]
        public List<LinkedModule> LinkedModules
        {
            get
            {
                return this.ProjectModule.LinkedModules;
            }
            set
            {
                this.ProjectModule.LinkedModules = value;
            }
        }

    }
}

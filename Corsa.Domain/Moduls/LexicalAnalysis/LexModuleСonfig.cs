using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Processing.Moduls.LexicalAnalysis
{
    [Table("LexModuleConfigs")]
    public class LexModuleСonfig: ModuleСonfig
    {
        public string Text { get; set; }
        
        public int? DataProviderId { get; set; }

        public string ExceptedWords { get; set; }

        public int? MinWordLenght { get; set; }

        public int? MaxWordLenght { get; set; }

        public int? HttpModuleId { get; set; }

        public int RequestAttempt { get; set; }

        [ForeignKey("DataProviderId")]
        public ProjectModule DataProvider { get; set; }

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

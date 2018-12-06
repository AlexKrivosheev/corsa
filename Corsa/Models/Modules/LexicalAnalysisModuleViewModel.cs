using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Processing.Moduls.LexicalAnalysis;
using Corsa.Domain.Tasks;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class LexicalAnalysisModuleViewModel : ModuleViewModel<LexModuleСonfig, LexModuleData>
   {
        public List<ModuleViewModel> DataProviders { get; set; }

        public List<ModuleViewModel> HttpModules { get; set; } = new List<ModuleViewModel>();

        public List<ModuleViewModel> AvailableLinkedModules { get; set; } = new List<ModuleViewModel>();

        public LexicalAnalysisModuleViewModel(int id, LexModuleСonfig config, List<ModuleTaskResult<LexModuleData>> results) : base(id, config, results)
        {

        }

        public LexicalAnalysisModuleViewModel(int id, LexModuleСonfig config) : base(id,config)
        {

        }

        public LexicalAnalysisModuleViewModel(LexModuleСonfig config):base(config)
        {

        }

        public LexicalAnalysisModuleViewModel() : base(new LexModuleСonfig())
        {

        }

        public string Text
        {
            get
            {
                return Config.Text;
            }
            set
            {
                Config.Text = value;
            }
        }

        public int Project
        {
            get
            {
                return Config.ProjectModule.ProjectId;
            }
            set
            {
                Config.ProjectModule.ProjectId = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return Config.ProjectModule.Project?.Name;
            }            
        }

        public string Name
        {
            get
            {
                return Config.ProjectModule.Name;
            }
            set
            {
                Config.ProjectModule.Name = value;
            }
        }

        public int? DataProvider
        {
            get
            {
                return Config.DataProviderId;
            }
            set
            {
                Config.DataProviderId = value;
            }
        }

        public string Description
        {
            get
            {
                return Config.ProjectModule.Description;
            }
            set
            {
                Config.ProjectModule.Description = value;
            }
        }

        public int? HttpModule
        {
            get
            {
                return Config.HttpModuleId;
            }
            set
            {
                Config.HttpModuleId = value;
            }
        }


        public int RequestAttempt
        {
            get
            {
                return Config.RequestAttempt;
            }
            set
            {
                Config.RequestAttempt = value;
            }
        }

        public List<DailySchedule> DailySchedules
        {
            get { return Config.DailySchedules; }
            set { Config.DailySchedules = value; }
        }

        public List<OneTimeSchedule> OneTimeSchedules
        {
            get { return Config.OneTimeSchedules; }
            set { Config.OneTimeSchedules = value; }
        }

        public List<LinkedModule> LinkedModules
        {
            get
            {
                return Config.ProjectModule.LinkedModules;
            }
            set
            {
                Config.ProjectModule.LinkedModules = value;
            }
        }

    }
}

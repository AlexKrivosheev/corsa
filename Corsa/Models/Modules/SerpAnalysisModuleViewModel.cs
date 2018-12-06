using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.SerpAnalysis;
using Corsa.Domain.Tasks;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class SerpAnalysisModuleViewModel : ModuleViewModel<SerpModuleConfig, SerpModuleData>
   {
        public List<ModuleViewModel> DataProviders { get; set; } = new List<ModuleViewModel>();

        public List<ModuleViewModel> AvailableLinkedModules { get; set; } = new List<ModuleViewModel>();
        
        public SerpAnalysisModuleViewModel(int id, SerpModuleConfig config, List<ModuleTaskResult<SerpModuleData>> results) : base(id, config, results)
        {

        }

        public SerpAnalysisModuleViewModel(int id, SerpModuleConfig config) : base(id,config)
        {

        }

        public SerpAnalysisModuleViewModel(SerpModuleConfig config):base(config)
        {

        }

        public SerpAnalysisModuleViewModel() : base(new SerpModuleConfig())
        {

        }

        public int Id
        {
            get
            {
                return Config.ProjectModule.Id;
            }
            set
            {
                Config.ProjectModule.Id = value;
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


        public string ProjectName
        {
            get
            {
                return Config.ProjectModule.Project?.Name;
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

        public string Name
        {
            get
            {
                return Config.ProjectModule?.Name;
            }
            set
            {
                Config.ProjectModule.Name = value;
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

        public string Url
        {
            get
            {
                return Config.Url;
            }
            set
            {
                Config.Url = value;
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

        public List<SerpModuleRequest> Requests
        {
            get { return Config.Requests; }
            set { Config.Requests = value; }
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
    }
}

using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.Providers.YandexXml;
using Corsa.Domain.Processing.Serp;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class YanexDirectModuleViewModel : ModuleViewModel<YandexDirectConfig, List<SerpWebPage>>
   {
        public List<ModuleViewModel> HttpModules { get; set; } = new List<ModuleViewModel>();

        public List<ModuleViewModel> AntigateModules { get; set; } = new List<ModuleViewModel>();

        public YanexDirectModuleViewModel(int id, YandexDirectConfig config, List<ModuleTaskResult<List<SerpWebPage>>> results) : base(id, config, results)
        {

        }

        public YanexDirectModuleViewModel(int id, YandexDirectConfig config) : base(id,config)
        {

        }

        public YanexDirectModuleViewModel(YandexDirectConfig config):base(config)
        {

        }

        public YanexDirectModuleViewModel() : base(new YandexDirectConfig())
        {

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

        public int? PageLimit
        {
            get
            {
                return Config.PageLimit;
            }
            set
            {
                Config.PageLimit = value;
            }
        }

        public string Region
        {
            get
            {
                return Config.Region;
            }
            set
            {
                Config.Region = value;
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

        public int? AntigateId
        {
            get
            {
                return Config.AntigateId;
            }
            set
            {
                Config.AntigateId = value;
            }
        }
    }
}

using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Moduls.Providers.YandexXml;
using Corsa.Domain.Processing.Serp;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
   public class YanexXMLModuleViewModel : ModuleViewModel<YandexXmlConfig, List<SerpWebPage>>
   {
        public List<ModuleViewModel> HttpModules { get; set; } = new List<ModuleViewModel>();
        public YanexXMLModuleViewModel(int id, YandexXmlConfig config, List<ModuleTaskResult<List<SerpWebPage>>> results) : base(id, config, results)
        {

        }

        public YanexXMLModuleViewModel(int id, YandexXmlConfig config) : base(id,config)
        {

        }

        public YanexXMLModuleViewModel(YandexXmlConfig config):base(config)
        {

        }

        public YanexXMLModuleViewModel() : base(new YandexXmlConfig())
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



        public string User
        {
            get
            {
                return Config.User;
            }
            set
            {
                Config.User = value;
            }
        }

        public string Key
        {
            get
            {
                return Config.Key;
            }
            set
            {
                Config.Key = value;
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

        public string Filter
        {
            get
            {
                return Config.Filter;
            }
            set
            {
                Config.Filter = value;
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

    }
}

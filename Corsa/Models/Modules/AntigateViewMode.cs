using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.RutimeModuls.Antigate;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class AntigateModulViewModel : ModuleViewModel<AntigateConfig, AntigateTaskResult>
    {
        public List<ModuleViewModel> HttpModules { get; set; } = new List<ModuleViewModel>();

        public AntigateModulViewModel(int id, AntigateConfig config, List<ModuleTaskResult<AntigateTaskResult>> results) : base(id, config, results)
        {

        }

        public AntigateModulViewModel(int id, AntigateConfig config) : base(id,config)
        {

        }

        public AntigateModulViewModel(AntigateConfig config):base(config)
        {

        }

        public AntigateModulViewModel() : base(new AntigateConfig())
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

        public string LanguagePool
        {
            get
            {
                return Config.LanguagePool;
            }
            set
            {
                Config.LanguagePool = value;
            }
        }

        public string ClientKey
        {
            get
            {
                return Config.ClientKey;
            }
            set
            {
                Config.ClientKey = value;
            }
        }

        public int SoftId
        {
            get
            {
                return Config.SoftId;
            }
            set
            {
                Config.SoftId = value;
            }
        }

        public string CallbackUrl
        {
            get
            {
                return Config.CallbackUrl;
            }
            set
            {
                Config.CallbackUrl = value;
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

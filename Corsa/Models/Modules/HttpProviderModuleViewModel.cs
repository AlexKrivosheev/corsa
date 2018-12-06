using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class HttpProviderModuleViewModel : ModuleViewModel<HttpProviderConfig, string>
   {  
        public HttpProviderModuleViewModel(int id, HttpProviderConfig config, List<ModuleTaskResult<string>> results) : base(id, config, results)
        {

        }

        public HttpProviderModuleViewModel(int id, HttpProviderConfig config) : base(id,config)
        {

        }

        public HttpProviderModuleViewModel(HttpProviderConfig config):base(config)
        {

        }

        public HttpProviderModuleViewModel() : base(new HttpProviderConfig())
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
   
        public string ProxyHost
        {
            get
            {
                return Config.ProxyHost;
            }
            set
            {
                Config.ProxyHost = value;
            }
        }

        public string ProxyPort
        {
            get
            {
                return Config.ProxyPort;
            }
            set
            {
                Config.ProxyPort = value;
            }
        }

        public string ProxyUser
        {
            get
            {
                return Config.ProxyUser;
            }
            set
            {
                Config.ProxyUser = value;
            }
        }

        public string ProxyPassword
        {
            get
            {
                return Config.ProxyPassword;
            }
            set
            {
                Config.ProxyPassword = value;
            }
        }
    }
}

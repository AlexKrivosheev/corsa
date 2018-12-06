using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Moduls.RutimeModuls;
using Corsa.Domain.Processing.Serp;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    public class YandexXMLModule : Module, IRuntimeModule<string, List<SerpWebPage>>, IConfigModule<YandexXmlConfig>
    {     
        public override string Name
        {
            get { return Configuration?.ProjectModule?.Name; }
        }
        
        public YandexXmlConfig Configuration { get; set; }

        public List<ModuleTaskResult<List<SerpWebPage>>> Results { get; set; }

        public YandexXMLModule()
        {

        }

        public YandexXMLModule(int id)
        {
            this.Id = id;
        }

        public static int ModuleCode = 1003;

        public override int Code
        {
            get { return ModuleCode; }            
        }

        public bool LoadConfig( )
        {
            Configuration = Context.Repository.GetYandexXmlConfig(Id) ?? new YandexXmlConfig();
            return true;
        }
        
        public UserActionDetails GetDetails(int id)
        {
            return Context.Repository.GetActionStatistic(id);
        }
   
        public bool Load( )
        {
            return LoadConfig();
        }

        public bool SaveConfig(YandexXmlConfig config)
        {
            var projectModule = Context.Repository.GetModule(Id);
            projectModule.Name = config.ProjectModule.Name;
            projectModule.Description = config.ProjectModule.Description;

            Configuration.PageLimit = 
            Configuration.ProjectModuleId = projectModule.Id;
            Configuration.User = config.User;
            Configuration.Key = config.Key;
            Configuration.Region = config.Region;            
            Configuration.Filter = config.Filter;
            Configuration.PageLimit = config.PageLimit;

            Configuration.HttpModuleId = config.HttpModuleId;
                         
            return Context.Repository.UpdateModule(projectModule) & Context.Repository.UpdateYandexXmlConfig(Configuration);
        }

        private Stream FetchData(IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, string request, int? page)
        {
            if (httpModule == null)
            {
                throw new NullReferenceException("Http module");
            }

            this.Context.LogInform(this.Context.Localizer[$"Request execution '{request}'"]);

            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.Append($"?user={Configuration.User}&key={Configuration.Key}&query={request}");

            if (!string.IsNullOrEmpty(Configuration.Region))
            {
                queryBuilder.Append($"&lr={Configuration.Region}");
            }

            if (!string.IsNullOrEmpty(Configuration.Filter))
            {
                queryBuilder.Append($"&filter={Configuration.Filter}");
            }

            if (page.HasValue&& page.Value>0)
            {
                queryBuilder.Append($"&page={page.Value}");
            }

            UriBuilder uri = new UriBuilder();
            uri.Host = "yandex.ru/search/xml";
            uri.Scheme = "http";
            uri.Query = queryBuilder.ToString() ;

            var result = RuntimeTask.Run(Context, httpModule, new HttpProviderRuntimeConfig() { Query = uri.ToString()});
            if (result.IsSuccessfully)
            {
                return result.Data.GetContent();   
            }

            throw new UserException(this.Context.Localizer[RuntimeTask.RuntimeException, httpModule.Name]);
        }

        public bool CreateAndSave(YandexXmlConfig config)
        {
            config.ProjectModule.Code = ModuleCode;
            config.ProjectModule.CreatedTime = DateTime.Now;
            config.ProjectModule.ProjectId = config.ProjectModule.ProjectId;
                                          
            return Context.Repository.AddModule(config.ProjectModule) & Context.Repository.AddYandexXmlConfig(config);            
        }

        public bool Drop()
        {
            return Context.Repository.DropModule(Id);
        }

        public List<SerpWebPage> Run(string config)
        {

            List<SerpWebPage> pages = new List<SerpWebPage>();

            int numberOfRequest = Configuration.PageLimit.HasValue ? Configuration.PageLimit.Value : 1;

            var registry = this.Context.Provider.GetService<IProjectModuleRegistry>();



            XmlYandexSerpParser yandexXMLParser = new XmlYandexSerpParser();

            IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule = null;

            if (Configuration.HttpModuleId.HasValue)
            {
                var targetModule = Context.Repository.GetModule(Configuration.HttpModuleId.Value);

                if (targetModule == null)
                {
                    throw new NullReferenceException("Http Module module");
                }

                httpModule = registry.OpenModule(Context, targetModule.Code, targetModule.Id) as IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData>;
            }

            for (int i = 0; i < numberOfRequest; i++)
            {
                using (var stream = FetchData(httpModule, config, i))
                {
                    var result = RuntimeTask.Run(Context, yandexXMLParser, stream);
                    if (result.IsSuccessfully)
                    {
                        pages.AddRange(result.Data);
                    }
                    else
                    {
                        throw new UserException(this.Context.Localizer[RuntimeTask.RuntimeException, yandexXMLParser.Name]);
                    }
                }
            }

            return pages;
        }   
    }
}

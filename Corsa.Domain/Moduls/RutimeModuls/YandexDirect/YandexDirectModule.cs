using Corsa.Domain.Models.Actions;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Moduls.RutimeModuls;
using Corsa.Domain.Processing.Serp;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Corsa.Domain.Moduls.RutimeModuls.Antigate;
using Corsa.Domain.Tasks;
using Corsa.Domain.Moduls.RutimeModuls.YandexDirect;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    public class YandexDirectModule : Module, IRuntimeModule<string, List<SerpWebPage>>, IConfigModule<YandexDirectConfig>
    {
        private YandexQueryGenerator QueryGenerator { get; set; } = new YandexQueryGenerator();

        public int Id { get; set; }

        public override string Name
        {
            get { return Configuration?.ProjectModule?.Name; }
        }

        public YandexDirectConfig Configuration { get; set; }

        public List<ModuleTaskResult<List<SerpWebPage>>> Results { get; set; }

        public YandexDirectModule()
        {

        }

        public YandexDirectModule(int id)
        {
            this.Id = id;
        }

        public static int ModuleCode = 1004;

        public override int Code
        {
            get { return ModuleCode; }
        }

        public bool LoadConfig()
        {
            Configuration = Context.Repository.GetYandexDirectConfig(Id) ?? new YandexDirectConfig();
            return true;
        }

        public UserActionDetails GetDetails(int id)
        {
            return Context.Repository.GetActionStatistic(id);
        }

        public bool Perform()
        {
            return false;
        }

        public bool Load()
        {
            return LoadConfig();
        }

        public bool SaveConfig(YandexDirectConfig config)
        {
            var projectModule = Context.Repository.GetModule(Id);
            projectModule.Name = config.ProjectModule.Name;
            projectModule.Description = config.ProjectModule.Description;

            Configuration.PageLimit =
            Configuration.ProjectModuleId = projectModule.Id;
            Configuration.AntigateId = config.AntigateId;
            Configuration.HttpModuleId = config.HttpModuleId;
            Configuration.Region = config.Region;
            Configuration.PageLimit = config.PageLimit;

            return Context.Repository.UpdateModule(projectModule) & Context.Repository.UpdateYandexDirectConfig(Configuration);
        }

        private List<SerpWebPage> Execute(string query, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, IRuntimeModule<AntigateTaskConfig, AntigateTaskResult> antigateModule)
        {
            if (httpModule == null) {
                throw new NullReferenceException("Http module");
            }

            if (antigateModule == null)
            {
                throw new NullReferenceException("Antigate module");
            }
            List<SerpWebPage> result = new List<SerpWebPage>();
            int pageLimit = Configuration.PageLimit.HasValue ? Configuration.PageLimit.Value : 1;

            YandexHtmlSerpParser parser = new YandexHtmlSerpParser();

            HtmlTagPageParser tagParser = new HtmlTagPageParser();

            for (int i = 0; i < pageLimit; i++)
            {
                var serpList = ExecuteRequest(new YandexQuery(query, i, Configuration.Region), parser, httpModule, antigateModule);
                result.AddRange(LoadSerpItemDetails(tagParser, httpModule, serpList));
            }

            return result;
        }

        private void TryExtractDetaisl(HtmlTagPageParser tagParser, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, string url, out string redirectUrl, out string title)
        {
            redirectUrl = string.Empty;
            title = string.Empty;

            var httpResult = RuntimeTask.Run(Context, httpModule, new HttpProviderRuntimeConfig() { Query = url });

            if (httpResult.IsSuccessfully)
            {
                var extractor = new HtmlTextAttributeParser()
                {
                    Attribute = "content",
                    PostHandle = (value) =>
                    {
                        string urlTag = "URL=";
                        int startIndex = value.IndexOf(urlTag);
                        if (startIndex != -1)
                        {
                            value = value.Substring(startIndex + urlTag.Length, value.Length - (urlTag.Length + startIndex));
                            value = value.Trim('\'');
                        }
                        return value;
                    }
                };

                var tagConfig = new HtmlTagPageConfig();
                tagConfig.Tags.Add("redirecturl", new SinglHtmlNodeSelector() { Tag = new TagProperties() { Path = @"//meta[@http-equiv='refresh']", Extractor = extractor } });
                tagConfig.Tags.Add("title", new SinglHtmlNodeSelector() { Tag = new TagProperties() { Path = @"//title", Extractor = new HtmlTagInnerText() } });

                using (var stream = httpResult.Data.GetContent())
                {
                    tagConfig.Stream = stream;

                    var parseResult = RuntimeTask.Run(Context, tagParser, tagConfig);
                    if (parseResult.IsSuccessfully)
                    {
                        if (parseResult.Data.Values["redirecturl"] != null)
                        {
                            redirectUrl = (string)parseResult.Data.Values["redirecturl"];
                        }

                        if (parseResult.Data.Values["title"] != null)
                        {
                            title = (string)parseResult.Data.Values["title"];
                        }
                    }
                }

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    httpResult = RuntimeTask.Run(Context, httpModule, new HttpProviderRuntimeConfig() { Query = redirectUrl });
                    if (httpResult.IsSuccessfully)
                    {
                        using (var stream = httpResult.Data.GetContent())
                        {
                            tagConfig.Stream = stream;
                            var tagResult = tagParser.Run(tagConfig);

                            if (tagResult.Values["title"] != null)
                            {
                                title = (string)tagResult.Values["title"];
                            }
                        }
                    }

                }
            }

        }

        private List<SerpWebPage> LoadSerpItemDetails(HtmlTagPageParser tagParser, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, List<YandexHtmlSerpItem> serpList)
        {
            List<SerpWebPage> result = new List<SerpWebPage>();
   
            foreach (var serpItem in serpList)
            {
                var targetUrl = serpItem.Href;
                var title = string.Empty;

                TryExtractDetaisl(tagParser, httpModule, serpItem.Href, out targetUrl, out title);
                
                SerpWebPage webPage = new SerpWebPage();
                webPage.Position = serpItem.Postion;

                webPage.Url = string.IsNullOrEmpty(targetUrl) ? serpItem.Href: targetUrl;
                webPage.Title = title;
                result.Add(webPage);
            }

            return result;
        }

        private AntigateTaskResult<ImageToTextTaskSolution> ResolveCaptcha(string captchaUrl, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, IRuntimeModule<AntigateTaskConfig, AntigateTaskResult> antigateModule)
        {
            string imageBase64 = FethcCaptcha(httpModule, captchaUrl);
            var task = new ImageToTextTask();
            task.Body = imageBase64;

            return (AntigateTaskResult<ImageToTextTaskSolution>)antigateModule.Run(task);
        }

        private List<YandexHtmlSerpItem> ExecuteRequest(YandexQuery query, YandexHtmlSerpParser parser, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, IRuntimeModule<AntigateTaskConfig, AntigateTaskResult> antigateModule)
        {           
            List<YandexHtmlSerpItem> result = new List<YandexHtmlSerpItem>();

            try
            {
                result = ExecuteRequest(query.Generate(QueryGenerator), parser, httpModule, antigateModule);
            }
            catch (CaptchaException exc)
            {
                var captchaResult = ResolveCaptcha(exc.ImageUrl, httpModule, antigateModule);

                if (string.IsNullOrEmpty(captchaResult.ErrorCode))
                {
                    result = ExecuteRequest(new YandexCheckCaptchaQuery(query.Query, query.Page, query.Region, exc.Key, captchaResult.Solution.Text, exc.Retpath), parser, httpModule, antigateModule);
                }
            }

            return result;
        }

        private string FethcCaptcha(IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, string query)
        {
            if (httpModule == null)
            {
                throw new NullReferenceException("Http module");
            }

            using (var stream = httpModule.Run(new HttpProviderRuntimeConfig() { Query = query }).GetContent())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        private List<YandexHtmlSerpItem> ExecuteRequest(string query, YandexHtmlSerpParser parser, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, IRuntimeModule<AntigateTaskConfig, AntigateTaskResult> antigateModule)
        {
            List<YandexHtmlSerpItem> result = new List<YandexHtmlSerpItem>();

            var httpResult = RuntimeTask.Run(Context, httpModule, new HttpProviderRuntimeConfig() { Query = query });
            if (httpResult.IsSuccessfully)
            {
                using (var stream = httpResult.Data.GetContent())
                {
                    var parserResult = RuntimeTask.Run(Context, parser, stream);
                    if (!parserResult.IsSuccessfully)
                    {
                        throw parserResult.Error;
                    }

                    result.AddRange(parserResult.Data);

                }
            }   
            return result;
        }

        private Stream FetchData(string request, ProxySetting proxy)
        {
            this.Context.LogInform(this.Context.Localizer[$"Request execution '{request}'"]);

            HttpProviderModule httpProvider = new HttpProviderModule();
            httpProvider.Context = this.Context;

            var config = new HttpProviderConfig();

            return httpProvider.Run(new HttpProviderRuntimeConfig() { Query = request }).GetContent();    
        }

        public bool CreateAndSave(YandexDirectConfig config)
        {
            config.ProjectModule.Code = ModuleCode;
            config.ProjectModule.CreatedTime = DateTime.Now;
            config.ProjectModule.ProjectId = config.ProjectModule.ProjectId;
                                          
            return Context.Repository.AddModule(config.ProjectModule) & Context.Repository.AddYandexDirectConfig(config);            
        }

        public bool Drop()
        {
            return Context.Repository.DropModule(Id);
        }

        public List<SerpWebPage> Run(string config)
        {
            List<SerpWebPage> result = new List<SerpWebPage>();

            int numberOfRequest = Configuration.PageLimit.HasValue ? Configuration.PageLimit.Value : 1;

            var registry = this.Context.Provider.GetService<IProjectModuleRegistry>();

            IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule = null;

            if (Configuration.HttpModuleId.HasValue)
            {
                var targetModule = Context.Repository.GetModule(Configuration.HttpModuleId.Value);

                if (targetModule == null)
                {
                    throw new NullReferenceException("Antigate module");
                }

                httpModule = registry.OpenModule(Context, targetModule.Code, targetModule.Id) as IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData>;
            }

            IRuntimeModule<AntigateTaskConfig, AntigateTaskResult> antigateModule = null;

            if (Configuration.AntigateId.HasValue)
            {
                var targetModule = Context.Repository.GetModule(Configuration.AntigateId.Value);

                if (targetModule == null)
                {
                    throw new NullReferenceException("Http module");
                }

                antigateModule = registry.OpenModule(Context, targetModule.Code, targetModule.Id) as IRuntimeModule<AntigateTaskConfig, AntigateTaskResult>;                
            }

            var serpList = Execute(config, httpModule, antigateModule);

            return serpList;
        }
    }
}

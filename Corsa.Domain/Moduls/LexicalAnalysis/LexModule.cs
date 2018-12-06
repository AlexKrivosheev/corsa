using System;
using Corsa.Domain.Processing.Moduls.LexicalAnalysis;
using Corsa.Domain.Processing.Moduls.RutimeModuls;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Models.SearchEngines;
using Corsa.Domain.Models.Projects;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Corsa.Domain.Moduls;
using Corsa.Domain.Exceptions;
using Corsa.Domain.Processing.Serp;
using System.IO;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Tasks;

namespace Corsa.Domain.Processing.Moduls
{
    public class LexModule : ProjectModule<LexModuleСonfig, LexModuleData>
    {
        public static int ModuleCode = 1001;

        public override int Code {
            get { return ModuleCode; }
        }

        public LexModule(int id)
        {
            this.Id = id;
        }

        public LexModule() : this(int.MinValue)
        {

        }

        

        private LexModuleStats Analyze(Request request)
        {
            if (Configuration.DataProvider == null)
            {
                throw new UserException("Data provider not specified");
            }
            var registry = this.Context.Provider.GetService<IProjectModuleRegistry>();

            var module = registry.OpenModule(Context, Configuration.DataProvider.Code, Configuration.DataProvider.Id);

            var dataProvider = module as IRuntimeModule<string, List<SerpWebPage>>;
            if (dataProvider == null)
            {
                throw new UserException("Data provider is incorrected");
            }

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

            ModuleTaskResult<List<SerpWebPage>> dataResult;

            var iteration = 0;

            do
            {
                iteration++;
                if (iteration > 1)
                {
                    Context.LogInform(Context.Localizer[$"The attemp {iteration} to get position of '{request.Text}'"]);
                }

                dataResult = RuntimeTask.Run(this.Context, dataProvider, request.Text);

                if (dataResult.IsSuccessfully)
                {
                    break;
                }

            } while (iteration <= Configuration.RequestAttempt);

            if (!dataResult.IsSuccessfully)
            {
                throw new UserException(this.Context.Localizer[RuntimeTask.RuntimeException, httpModule.Name]);
            }

            FillSerpPageDetails(httpModule, dataResult.Data);

            var analyser = new MedianFrequencyAnalyser();

            var result = RuntimeTask.Run(Context, analyser, new MedianFrequencyAnalyserConfig() { Pages = dataResult.Data, Request = request, HttpModule = httpModule });
            if (result.IsSuccessfully)
            {
                return result.Data;
            }

            throw new UserException(this.Context.Localizer[RuntimeTask.RuntimeException, analyser.Name]);
        }

        private void FillSerpPageDetails(IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, List<SerpWebPage> pages)
        {
            if (httpModule == null)
            {
                throw new NullReferenceException("httpModule");
            }

            foreach (var page in pages)
            {
                try
                {
                    var serpUrl = new Uri(page.Url);
                    var targetBuilder = new UriBuilder(serpUrl.Scheme, serpUrl.Host, serpUrl.Port, "favicon.ico");

                    var result = RuntimeTask.Run(Context, httpModule, new HttpProviderRuntimeConfig() { Query = targetBuilder.Uri.ToString() });

                    if (result.IsSuccessfully)
                    {
                        using (var strean = result.Data.GetContent())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                strean.CopyTo(ms);
                                page.Favicon = ms.ToArray();
                            }
                        }
                    }
                    
                }
                catch (Exception exc)
                {

                }
            }
        }

        public override bool LoadConfig()
        {
           Configuration = Context.Repository.GetLexModuleConfig(Id);
            return true;
        }

        public override bool CreateAndSave(LexModuleСonfig config)
        {
            var projectModul = new ProjectModule() { Code = ModuleCode,  CreatedTime = DateTime.Now, ProjectId = config.ProjectModule.ProjectId, Name = config.ProjectModule.Name, Description = config.ProjectModule.Description };
            Context.Repository.AddModule(projectModul);            
            config.ProjectModule = projectModul;

            Context.Repository.AddLexModuleConfig(config);

            return true;
        }

        public override bool SaveConfig(LexModuleСonfig config)
        {
            var projectModule = Context.Repository.GetModule(Id);
            projectModule.Name = config.ProjectModule.Name;
            projectModule.Description = config.ProjectModule.Description;

            Configuration.Text = config.Text;
            Configuration.HttpModuleId = config.HttpModuleId;
            Configuration.RequestAttempt = config.RequestAttempt;
            Configuration.DataProviderId = config.DataProviderId;

            Configuration.LinkedModules = MergeCollection(config.LinkedModules, Configuration.LinkedModules, (item1, item2) => item1.LinkedProjectModuleId == item2.LinkedProjectModuleId, (item) => { item.ProjectModuleId = Configuration.Id; });
            Configuration.OneTimeSchedules = MergeCollection(config.OneTimeSchedules, Configuration.OneTimeSchedules, (item1, item2) => item1.DateTime == item2.DateTime, (item) => { item.ProjectModuleId = Id; });
            Configuration.DailySchedules = MergeCollection(config.DailySchedules, Configuration.DailySchedules, (item1, item2) => item1.Day == item1.Day && item1.Time == item2.Time, (item) => { item.ProjectModuleId = Id; });

            return Context.Repository.UpdateModule(projectModule) & Context.Repository.UpdateLexModuleConfig(config);
        }

        public override bool LoadResults()
        {
            Results = new List<ModuleTaskResult<LexModuleData>>();
            foreach (var moduleResult in Context.Repository.GetProjectModuleResults(Id))
            {
                ModuleTaskResult<LexModuleData> lexModuleResult = new ModuleTaskResult<LexModuleData>();
                lexModuleResult.Id = moduleResult.Id;
                lexModuleResult.Details = moduleResult.Stats;
                lexModuleResult.Data = new LexModuleData() { Stats = Context.Repository.GetLexModuleStats(Configuration.Id).FirstOrDefault()};
                Results.Add(lexModuleResult);
            }
            
            return true;
        }

        public override bool SaveResult(ModuleTaskResult<LexModuleData> result)
        {
            Context.Repository.DropLexModuleStats(Configuration.Id);
            Context.Repository.ClearModuleResults(Id);

            ProjectModuleResult moduleResult = new ProjectModuleResult();
            moduleResult.Stats = result.Details;
            moduleResult.ProjectModuleId = Id;

            Context.Repository.AddModuleResults(moduleResult);

            LexModuleStats resultData = new LexModuleStats()
            {
                LexModuleConfigId = Configuration.Id,                
                SerpPages = result.Data?.Stats?.SerpPages,
                Words = result.Data?.Stats?.Words,
                FrequencyData = result.Data?.Stats?.FrequencyData
            };

            Context.Repository.AddLexModuleStats(resultData);
            return true;
        }
       
        public override LexModuleData Run()
        {
            LexModuleData data = new LexModuleData();
            var statistics = Analyze(new Request() { Text = Configuration.Text });
            data.Stats = statistics;
            return data;
        }
    }
}

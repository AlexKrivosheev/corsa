using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Serp;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Corsa.Domain.Tasks;

namespace Corsa.Domain.Moduls.SerpAnalysis
{

    public class SerpModule : ProjectModule<SerpModuleConfig,SerpModuleData>
    {
        public SerpModule(int id)
        {
            Id = id;   
        }

        public const int ModuleCode = 1002;

        public override int Code
        {
            get { return ModuleCode; }
        }

        public SerpModule() : this(int.MinValue)
        {

        }

        public override bool CreateAndSave(SerpModuleConfig config)
        {
            config.ProjectModule.Code = ModuleCode;
            config.ProjectModule.CreatedTime = DateTime.Now;
            config.ProjectModule.ProjectId = config.ProjectModule.ProjectId;
            Context.Repository.AddModule(config.ProjectModule);

            return Context.Repository.AddSerpModuleConfig(config);
        }

        public override bool LoadConfig()
        {
            Configuration = Context.Repository.GetSerpModuleConfig(Id);
            return true;
        }

        public override bool LoadResults()
        {
            Results = new List<ModuleTaskResult<SerpModuleData>>();

            foreach (var modulResult in Context.Repository.GetProjectModuleResults(Id))
            {
                ModuleTaskResult<SerpModuleData> serpModuleResult = new ModuleTaskResult<SerpModuleData>();
                serpModuleResult.Id = modulResult.Id;
                serpModuleResult.Details = modulResult.Stats;
                serpModuleResult.Data = new SerpModuleData() { RequestStats = new List<SerpModuleRequestStats>(Context.Repository.GetSerpModuleRequestStats(modulResult.Id)) };
                Results.Add(serpModuleResult);
            }
            return true;
        }

        public override bool SaveConfig(SerpModuleConfig config)
        {
            var projectModule = Context.Repository.GetModule(Id);
            projectModule.Name = config.ProjectModule.Name;
            projectModule.Description = config.ProjectModule.Description;

            Configuration.Url = config.Url;
            Configuration.DataProviderId = config.DataProviderId;
            Configuration.RequestAttempt = config.RequestAttempt;

            List<SerpModuleRequest> requests = new List<SerpModuleRequest>();

            Configuration.Requests = MergeCollection(config.Requests, Configuration.Requests, (item1, item2) => string.Equals(item1.Text, item2.Text, StringComparison.InvariantCultureIgnoreCase), (item) => { item.SerpModuleConfigId = Configuration.Id; });
            Configuration.LinkedModules = MergeCollection(config.LinkedModules, Configuration.LinkedModules, (item1, item2) => item1.LinkedProjectModuleId == item2.LinkedProjectModuleId, (item) => { item.ProjectModuleId = Configuration.Id; });
            Configuration.OneTimeSchedules = MergeCollection(config.OneTimeSchedules, Configuration.OneTimeSchedules, (item1, item2) => item1.DateTime == item2.DateTime, (item) => { item.ProjectModuleId = Id; });
            Configuration.DailySchedules = MergeCollection(config.DailySchedules, Configuration.DailySchedules, (item1, item2) => item1.Day == item1.Day && item1.Time == item2.Time, (item) => { item.ProjectModuleId = Id; });

            return Context.Repository.UpdateModule(projectModule) & Context.Repository.UpdateSerpModuleConfig(Configuration);
        }

        public override bool SaveResult(ModuleTaskResult<SerpModuleData> result)
        {
            ProjectModuleResult moduleResult = new ProjectModuleResult();
            moduleResult.Stats = result.Details;
            moduleResult.ProjectModuleId = Id;

            Context.Repository.AddModuleResults(moduleResult);

            if (result.Data != null)
            {
                foreach (var item in result.Data.RequestStats)
                {
                    item.ProjectModuleResultId = moduleResult.Id;
                    Context.Repository.AddSerpModuleRequestStats(item);
                }
            }
            return true;
        }

        private SerpModuleRequestStats FindRequestPosition(SerpModuleRequest request, IRuntimeModule<string, List<SerpWebPage>> dataProvider, string targetPage)
        {
            var requestStats = new SerpModuleRequestStats();

            Context.LogInform(Context.Localizer[$"Analysis position of '{request.Text}' request..."]);

            var result = RuntimeTask.Run(this.Context, dataProvider, request.Text);

            if (result.IsSuccessfully)
            {
                var target = result.Data.Where(page => page.Url.Contains(targetPage.ToLower())).SingleOrDefault();
                if (target != null)
                {
                    requestStats.Request = request;
                    requestStats.Position = target.Position;
                    requestStats.DetectionTime = DateTime.Now;

                    Context.LogInform(Context.Localizer[$"Request '{request.Text}' found. Position : {target.Position}"]);
                }
                else
                {
                    Context.LogInform(Context.Localizer[$"Request '{request.Text}' not found"]);
                }

                Context.LogInform(Context.Localizer[$"Analysis position of '{request.Text}' request was completed"]);
            }

            throw new UserException(this.Context.Localizer[RuntimeTask.RuntimeException, dataProvider.Name]);            
        }

        private List<SerpModuleRequestStats> Analyze()
        {
            List<SerpModuleRequestStats> stats = new List<SerpModuleRequestStats>();
     
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

            foreach (var request in Configuration.Requests)
            {

                var iteration = 0;

                SerpModuleRequestStats requestResult = null;

                do
                {
                    iteration++;
                    if (iteration > 1)
                    {
                        Context.LogInform(Context.Localizer[$"The attemp {iteration} to get position of '{request.Text}'"]);
                    }

                    try
                    {
                        requestResult = FindRequestPosition(request, dataProvider, Configuration.Url);
                    }
                    catch (UserException exc)
                    {                                                
                    }
                } while (iteration <= Configuration.RequestAttempt);

                
                stats.Add(requestResult ?? new SerpModuleRequestStats() { Position = -1, DetectionTime = DateTime.Now, Request = request, Details = Context.Localizer[$"An error occured during getting of position the request. See log"]});

            }

            return stats;
        }

        public override SerpModuleData Run()
        {
            SerpModuleData data = new SerpModuleData();
            data.RequestStats.AddRange(Analyze());
            return data;
        }
    }
}

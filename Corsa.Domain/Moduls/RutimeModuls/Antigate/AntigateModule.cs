using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.RutimeModuls.Antigate;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Runtime.Serialization;
using Corsa.Domain.Tasks;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{ 
    public class AntigateModule : Module, IRuntimeModule<AntigateTaskConfig, AntigateTaskResult>, IConfigModule<AntigateConfig>
    {
        public override string Name => this.Context.Localizer["Antigate"];

        public AntigateModule()
        {

        }

        public AntigateModule(int id)
        {
            this.Id = id;
        }

        public override int Code
        {
            get
            {
                return ModuleCode;
            }
        }

        public int Id { get; set; }

        public static int ModuleCode = 1006;

  

        public AntigateConfig Configuration { get; set; }

        public  AntigateTaskResult Run(AntigateTaskConfig config)
        {
            var task = LoadAsync(config);
            return task.Result;
        }

        private Stream ExecuteRequest(string query, IRuntimeModule<string, Stream> httpModule)
        {            
            using (var stream = httpModule.Run(query))
            {
                
                
            }
            return Stream.Null;
        }

        public async Task<AntigateTaskResult> LoadAsync(AntigateTaskConfig config)
        {
            AntigateTaskResult rsult = new AntigateTaskResult();

            var registry = this.Context.Provider.GetService<IProjectModuleRegistry>();

            IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule = null;

            if (Configuration.HttpModuleId.HasValue)
            {
                var targetModule = Context.Repository.GetModule(Configuration.HttpModuleId.Value);

                if (targetModule == null)
                {
                    throw new NullReferenceException("Http module");
                }

                httpModule = registry.OpenModule(Context,targetModule.Code, targetModule.Id) as IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData>;                
            }

            var createTaskResponse = CreatTask(config,httpModule);

            if (createTaskResponse.ErrorId == 0)
            {                
                do
                {
                    Thread.Sleep(5000);
                    rsult = GetTaskResult(httpModule, createTaskResponse, config.ResultType);
                } while (rsult.Status == "processing");
            }
            else
            {

            }

            rsult.Task = config;

            return rsult;
        }

        private string SerializeObjectToJson<TObject>(TObject content)
        {
            var settings = new DataContractJsonSerializerSettings();
            settings.EmitTypeInformation = EmitTypeInformation.Never;
            var ser = new DataContractJsonSerializer(typeof(TObject), settings);
            
            string jsonObject = "{}";
            using (var memoryStream = new MemoryStream())
            {
                ser.WriteObject(memoryStream, content);
                
                using (StreamReader sr = new StreamReader(memoryStream))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    jsonObject = sr.ReadToEnd();
                }
            }

            return jsonObject;
        }

        private TObject DeserializeJsonToObject<TObject>(Stream stream)
        {
            var ser = new DataContractJsonSerializer(typeof(TObject));
            return (TObject)ser.ReadObject(stream);
        }

        private TObject DeserializeJsonToObject<TObject>(Type resulType, Stream stream)
        {
            var ser = new DataContractJsonSerializer(resulType);
            return (TObject)ser.ReadObject(stream);
        }

        public AntigateCreateTaskResponse CreatTask(AntigateTaskConfig task, IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule)
        {
            var creatTaskObject = new AntigateCreateTaskRequest(Configuration.ClientKey, Configuration.LanguagePool, Configuration.SoftId, Configuration.CallbackUrl);
            creatTaskObject.Task = task;

            HttpProviderRuntimeConfig httpConfig = new HttpProviderRuntimeConfig();
            httpConfig.Query = "https://api.anti-captcha.com/createTask";
            httpConfig.IsPost = true;
            httpConfig.Content = new StringContent(SerializeObjectToJson(creatTaskObject), Encoding.UTF8, "application/json");
            
            var result = RuntimeTask.Run(Context,httpModule, httpConfig);
            if(result.Details.Result!= ActionExecutionResult.Error)
            {
                using (var stream = result.Data.GetContent())
                {
                    return DeserializeJsonToObject<AntigateCreateTaskResponse>(stream);
                }
            }

            return null;
        }

        public AntigateTaskResult GetTaskResult(IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData> httpModule, AntigateCreateTaskResponse createTaskResponse, Type resulType)
        {
            var creatTaskObject = new AntigateGetTaskResultRequest(Configuration.ClientKey,createTaskResponse.TaskId);
           
            HttpProviderRuntimeConfig httpConfig = new HttpProviderRuntimeConfig();
            httpConfig.Query = "https://api.anti-captcha.com/getTaskResult";
            httpConfig.IsPost = true;
            httpConfig.Content = new StringContent(SerializeObjectToJson(creatTaskObject), Encoding.UTF8, "application/json");

            using (var stream = httpModule.Run(httpConfig).GetContent())
            { 
                return DeserializeJsonToObject<AntigateTaskResult>(resulType,stream);
            }
        }

        public bool CreateAndSave(AntigateConfig config)
        {
            config.ProjectModule.Code = ModuleCode;
            config.ProjectModule.CreatedTime = DateTime.Now;
            config.ProjectModule.ProjectId = config.ProjectModule.ProjectId;

            return Context.Repository.AddModule(config.ProjectModule) & Context.Repository.AddAntigateConfig(config);
        }

        public UserActionDetails GetDetails(int id)
        {
            return Context.Repository.GetActionStatistic(id);
        }

        public bool SaveConfig(AntigateConfig config)
        {
            var projectModule = Context.Repository.GetModule(Id);
            projectModule.Name = config.ProjectModule.Name;
            projectModule.Description = config.ProjectModule.Description;            

            Configuration.ClientKey = config.ClientKey;
            Configuration.CallbackUrl = config.CallbackUrl;
            Configuration.LanguagePool = config.LanguagePool;
            Configuration.SoftId = config.SoftId;
            Configuration.HttpModuleId = config.HttpModuleId;

            return Context.Repository.UpdateModule(projectModule) & Context.Repository.UpdateAntigateConfig(Configuration);
        }

        public bool LoadConfig()
        {
            Configuration = Context.Repository.GetAntigateConfig(Id) ?? new AntigateConfig();
            return true;
        }

        public bool Load()
        {
            return LoadConfig();
        }

        public bool Drop()
        {
            return Context.Repository.DropModule(Id);
        }

    }
}

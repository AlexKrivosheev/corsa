using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Moduls;
using Corsa.Domain.Processing;
using Corsa.Domain.Processing.Moduls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Localization;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Models.Projects;

namespace Corsa.Domain.Tasks.Modules
{
    public class ProjectModuleTask<TConfig, TData> : ProjectModuleTask<TData>
    {
        public TConfig Config { get; set; }

        public ModuleTaskResult<TData> Result { get; set; }

        public ProjectModuleTask(IServiceProvider provider, int moduleId, string user, TConfig config) : base(provider, moduleId, user)
        {
            Config = config;
        }

        public override TData RunModule()
        {
            var runtimeModule = Module as IRuntimeModule<TConfig, TData>;
            if (runtimeModule == null)
            {
                throw new UserException("Invalid runtime module");
            }

            return runtimeModule.Run(Config);
        }

        public override bool Start()
        {
            try
            {
                OnStarted();

                using (Context = CreatContext())
                {
                    Result = base.Run();

                    OnCompleted(Result.Details.Result, Result.Details.Message);

                    return Result.Details.Result != ActionExecutionResult.Error;
                }
            }
            catch (UserException exc)
            {
                OnCompleted(ActionExecutionResult.Error, exc.Message);
            }
            catch (Exception exc)
            {
                OnCompleted(ActionExecutionResult.Error, this.Context.Localizer["Unexptected error. See logs"]);
            }
            return false;
        }
    }

    public class ProjectModuleTask<TData> : ModuleTask<TData>
    {
        public ProjectModuleTask(IServiceProvider provider, int moduleId, string user) : base(moduleId)
        {
            ModuleId = moduleId;
            User = user;
            Provider = provider;
        }

        public ProjectModuleTask(IServiceProvider provider, IProjectModule projectModule, string user) : base(projectModule.Id)
        {            
            User = user;
            Provider = provider;
        }

        public string User { get; set; }

        public IServiceProvider Provider { get; set; }

        public int ModuleId { get; set; }

        public IProjectModule Module { get; set; }

        public override string Name
        {
            get
            {
                return Module?.Name;
            }
        }

        public override int Code
        {
            get
            {
                if (Module != null)
                {
                    return Module.Code;
                }

                return -1;
            }
        }

        public RuntimeContext CreatContext()
        {
            var loggerFactory = Provider.GetService<ILoggerFactory>();
            var localizer = Provider.GetService<IStringLocalizer<ModuleTask<TData>>>();

            var logger = new ModuleTaskLogger(this, loggerFactory.CreateLogger("User"));

            return new RuntimeContext(Provider, Provider.GetService<ISourceRepository>(), logger, localizer, User);
        }

        public override ModuleTaskResult<TData> Run()
        {
            var result = new ModuleTaskResult<TData>();

            var registry = Provider.GetService<IProjectModuleRegistry>();

            var projectModule = Context.Repository.GetModule(this.ModuleId);
            if (projectModule == null)
            {
                throw new UserException(Context.Localizer[$"Config module {this.ModuleId} not found."]);
            }

            Module = registry.OpenModule(Context, projectModule.Code, projectModule.Id);
            return base.Run();
        }

        public override TData RunModule()
        {
            var runtimeModule = Module as IRuntimeModule<TData>;
            if (runtimeModule == null)
            {
                throw new UserException("Invalid runtime module");
            }
            return runtimeModule.Run();
        }

        public void InvokeLinkedModules(ModuleTaskResult<TData> result)
        {
            var linkableModule = Module as ILinkable;
            if (linkableModule != null)
            {
                var linkedModules = linkableModule.GetLinkedModules();
                var registry = Provider.GetService<IProjectModuleRegistry>();
                var repository = Provider.GetService<ISourceRepository>();
                var taskQueue = Provider.GetService<ITaskPipline>();
                foreach (var module in linkedModules)
                {
                    try
                    {
                        if (module.LinkedProjectModule == null)
                        {
                            this.Context.LogInform(this.Context.Localizer[$"Modul {module.Id} execution was not found and was skipped"]);
                            continue;
                        }

                        if (registry.GetModuleTask<ModuleTaskResult<TData>>(module.LinkedProjectModule.Code)!=null)
                        {
                            var moduleTask = registry.CreateModuleTask<ModuleTaskResult<TData>>(module.LinkedProjectModule.Code, new object[] { Provider, module.LinkedProjectModuleId ,Context.User, result });
                            taskQueue.Add(moduleTask);
                        }

                        if (registry.GetModuleTask<TData>(module.LinkedProjectModule.Code) != null)
                        {
                            var moduleTask = registry.CreateModuleTask<TData>(module.LinkedProjectModule.Code, new object[] { Provider, module.LinkedProjectModuleId, Context.User,result.Data });
                            taskQueue.Add(moduleTask);
                        }

                    }
                    catch (Exception exc)
                    {
                        this.Context.LogError(exc.Message, exc);
                    }
                }
            }
        }

        public override bool Start()
        {
            var result = new ModuleTaskResult<TData>();

            try
            {
                OnStarted();
                
                using (Context = CreatContext())
                {
                    result = Run();

                    var dataModule = Module as IDataModule<TData>;
                    if (dataModule == null)
                    {
                        throw new UserException("Invalid data module");
                    }

                    dataModule.SaveResult(result);

                    OnCompleted(result.Details.Result, result.Details.Message);

                    return result.IsSuccessfully;
                }
            }
            catch (UserException exc)
            {                
                OnCompleted(ActionExecutionResult.Error, exc.Message);
            }
            catch (Exception exc)
            {
                OnCompleted(ActionExecutionResult.Error, this.Context.Localizer["Unexptected error. See logs"]);
            }
            finally
            {
                InvokeLinkedModules(result);
            }
            return false;

        }
    }
}

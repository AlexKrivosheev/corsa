using Corsa.Domain.Exceptions;
using Corsa.Domain.Moduls;
using Corsa.Domain.Processing;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;

namespace Corsa.Models
{
    public class ProjectModuleRegistry : IProjectModuleRegistry
    {
        public Dictionary<int, IProjectModuleFactory> _registry = new Dictionary<int, IProjectModuleFactory>();

        public Dictionary<int, Type> _defaultModuleTask = new Dictionary<int, Type>();

        public Dictionary<int, Dictionary<Type, Type>> _runtimeModuleTasks = new Dictionary<int, Dictionary<Type, Type>>();

        public Dictionary<int, List<Type>> _linkedTypeModules = new Dictionary<int, List<Type>>();

        public TModul CreateModule<TModul>(RuntimeContext context, int code) where TModul : IProjectModule
        {
            IProjectModuleFactory factory;
            if (!_registry.TryGetValue(code, out factory))
            {
                throw new UserException($"Modul {code} isn't registered");
            }
            var modul = (TModul)factory.CreateModule();
            modul.Context = context;
            
            return modul;
        }

        public TModule OpenModule<TModule>(RuntimeContext context, int code, int id) where TModule : IProjectModule
        {
            return (TModule)OpenModule(context,code, id);
        }

        public Type GetModuleTask(int code)
        {
            Type type;
            if (!_defaultModuleTask.TryGetValue(code, out type))
            {
                throw new UserException($"Modul task {code} isn't registered");
            }

            return type;
        }

        public Type GetModuleTask<TParameter>(int code)
        {
            Dictionary<Type, Type> types;
            if (!_runtimeModuleTasks.TryGetValue(code, out types))
            {
                throw new UserException($"Modul task {code} isn't registered");
            }

            Type type = null;
            if (types != null)
            {
                types.TryGetValue(typeof(TParameter), out type);
            }

            return type;
        }

        public IProjectModule OpenModule(RuntimeContext context, int code, int id)
        {
            IProjectModuleFactory factory;
            if (!_registry.TryGetValue(code, out factory))
            {
                throw new UserException($"Modul {code} isn't registered");
            }

            var modul = factory.OpenModule(id);
            modul.Context = context;
            var projectModule = modul as IProjectModule;

            if (projectModule != null)
            {                
                projectModule.Load();
            }

            return modul;
        }

        public void RegisterModule(int code, IProjectModuleFactory factory, Type defaultModuleTask = null, Dictionary<Type, Type> runtimeModuleTasks = default(Dictionary<Type, Type>))
        {
            if (_registry.ContainsKey(code))
            {
                throw new UserException($"Modul {code} has registered allready");
            }

            _registry.Add(code, factory);
            _defaultModuleTask.Add(code, defaultModuleTask);
            _runtimeModuleTasks.Add(code, runtimeModuleTasks);
        }

        public ModuleTask CreateModuleTask(int code, params object[] args)
        {
            var type = GetModuleTask(code);
            if(type==null)
            {
                throw new UserException($"Default moduleTask task {code} isn't registered");
            }

            return (ModuleTask)Activator.CreateInstance(type, args);
        }

        public ModuleTask CreateModuleTask<TParameter>(int code, params object[] args)
        {
            var type = GetModuleTask<TParameter>(code);
            if (type == null)
            {
                throw new UserException($"Project moduleTask task {typeof(TParameter)} isn't registered");
            }
            return (ModuleTask)Activator.CreateInstance(type, args);
        }


        public static List<ModuleViewModel> GetModules<TParameter>(RuntimeContext context, IProjectModuleRegistry moduleRegistry, int projectId)
        {
            List<ModuleViewModel> result = new List<ModuleViewModel>();

            foreach (var projectModule in context.Repository.GetProjectModules(projectId))
            {
                if (moduleRegistry.GetModuleTask<TParameter>(projectModule.Code) != null)
                {
                    result.Add(new ModuleViewModel() { Id = projectModule.Id, Name = projectModule.Name });
                }
            }

            return result;
        }

        public static List<ModuleViewModel> GetModules<TConfig, TData>(RuntimeContext context, IProjectModuleRegistry moduleRegistry, int projectId)
        {
            List<ModuleViewModel> result = new List<ModuleViewModel>();

            foreach (var projectModule in context.Repository.GetProjectModules(projectId))
            {
                var targetModule = moduleRegistry.OpenModule(context, projectModule.Code, projectModule.Id);

                var dataProvider = targetModule as IRuntimeModule<TConfig, TData>;
                if (dataProvider != null)
                {
                    result.Add(new ModuleViewModel() { Id = projectModule.Id, Name = projectModule.Name });
                }
            }

            return result;
        }
    }
}

using Corsa.Domain.Moduls;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;

namespace Corsa.Domain.Processing.Moduls
{
    public interface IProjectModuleRegistry
    {
        void RegisterModule(int code, IProjectModuleFactory factory, Type defaultModuleTask = null, Dictionary<Type, Type> runtimeModuleTasks = default(Dictionary<Type, Type>));

        TModule CreateModule<TModule>(RuntimeContext context, int code) where TModule : IProjectModule;

        TModule OpenModule<TModule>(RuntimeContext context, int code, int id) where TModule : IProjectModule;

        IProjectModule OpenModule(RuntimeContext context,int code, int id);

        Type GetModuleTask(int code);

        Type GetModuleTask<TParameter>(int code);

        ModuleTask CreateModuleTask(int code, params object[] args);

        ModuleTask CreateModuleTask<TParameter>(int code, params object[] args);
    }
}

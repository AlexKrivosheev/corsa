using Corsa.Domain.Moduls;
using Corsa.Domain.Processing;
using Corsa.Domain.Processing.Moduls;
using System;


namespace Corsa.Domain.Tasks.Modules
{
    public class RuntimeModuleTask<TConfig, TData> : ModuleTask<TData>
    {
        public IModule Module { get; set; }

        public TConfig Config { get; set; }

        public override string Name
        {
            get
            {
                return Module.Name;
            }
        }

        public override int Code
        {
            get
            {
                return Module.Code;
            }
        }

        public RuntimeModuleTask(RuntimeContext context, IModule module, TConfig config) : base(context, module.Id)
        {
            this.Context = context;
            this.Module = module;
            this.Config = config;
        }

        public override TData RunModule()
        {
            var runtimeModule = this.Module as IRuntimeModule<TConfig, TData>;
            if (runtimeModule != null)
            {
                return runtimeModule.Run(Config);
            }

            return default(TData);
        }

        public override bool Start()
        {
            throw new NotImplementedException();
        }
    }
}

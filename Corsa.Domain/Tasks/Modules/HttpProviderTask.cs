using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using System;
using System.IO;


namespace Corsa.Domain.Tasks.Modules
{
    public class HttpProviderTask : ProjectModuleTask<HttpProviderRuntimeConfig, string>
    {
        public HttpProviderTask(IServiceProvider provider, int moduleId, string user, HttpProviderRuntimeConfig config) : base(provider, moduleId, user, config)
        {

        }

        public override string RunModule()
        {
            var runtimeModule = Module as IRuntimeModule<HttpProviderRuntimeConfig, HttpProviderData>;
            if (runtimeModule != null)
            {
                var result = runtimeModule.Run(Config);

                using (StreamReader reader = new StreamReader(runtimeModule.Run(Config).GetContent()))
                {
                    return reader.ReadToEnd();
                }
            }

            return string.Empty;
        }


    }
}

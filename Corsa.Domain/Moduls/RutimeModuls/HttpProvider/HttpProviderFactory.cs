using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Moduls.RutimeModuls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    public class HttpProviderFactory : IProjectModuleFactory
    {
        public IProjectModule CreateModule()
        {
            return new HttpProviderModule();
        }

        public IProjectModule OpenModule(int id)
        {
            return new HttpProviderModule(id);
        }
    }
}

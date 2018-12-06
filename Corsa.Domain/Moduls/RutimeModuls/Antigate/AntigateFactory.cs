using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Moduls.RutimeModuls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    public class AntigateFactory : IProjectModuleFactory
    {
        public IProjectModule CreateModule()
        {
            return new AntigateModule();
        }

        public IProjectModule OpenModule(int id)
        {
            return new AntigateModule(id);
        }
    }
}

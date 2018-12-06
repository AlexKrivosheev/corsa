using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    public class YandexDirectFactory : IProjectModuleFactory
    {
        public IProjectModule CreateModule()
        {
            return new YandexDirectModule();
        }

        public IProjectModule OpenModule(int id)
        {
            return new YandexDirectModule(id);
        }
    }
}

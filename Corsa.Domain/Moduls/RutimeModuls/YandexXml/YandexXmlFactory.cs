using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    public class YandexXmlFactory : IProjectModuleFactory
    {
        public IProjectModule CreateModule()
        {
            return new YandexXMLModule();
        }

        public IProjectModule OpenModule(int id)
        {
            return new YandexXMLModule(id);
        }
    }
}

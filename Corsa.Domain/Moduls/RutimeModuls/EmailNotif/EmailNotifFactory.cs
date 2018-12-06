using Corsa.Domain.Moduls.RutimeModuls.EmailNotif;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Moduls.RutimeModuls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    public class EmailNotifFactory : IProjectModuleFactory
    {
        public IProjectModule CreateModule()
        {
            return new EmailNotifModule();
        }

        public IProjectModule OpenModule(int id)
        {
            return new EmailNotifModule(id);
        }
    }
}

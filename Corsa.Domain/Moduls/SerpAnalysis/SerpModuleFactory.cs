using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.SerpAnalysis
{
    public class SerpModuleFactory : IProjectModuleFactory
    {
        public IProjectModule CreateModule()
        {
            return new SerpModule();
        }

        public IProjectModule OpenModule(int id)
        {
            return new SerpModule(id);
        }
    }
}

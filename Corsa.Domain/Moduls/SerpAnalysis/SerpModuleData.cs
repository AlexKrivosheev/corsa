using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.SerpAnalysis
{
    public class SerpModuleData : ModuleData
    {
        public List<SerpModuleRequestStats> RequestStats { get; set; } = new List<SerpModuleRequestStats>();

    }
}

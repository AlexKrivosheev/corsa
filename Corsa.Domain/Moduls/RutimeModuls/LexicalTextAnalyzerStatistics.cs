using Corsa.Domain.Processing.Serp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Processing.Moduls.RutimeModuls
{
    public class LexicalTextAnalyzerStatistics
    {
        public SerpWebPage Page { get; set; }

        public int TotalWords { get; set; }

        public Dictionary<string, int> WordStatistics { get; set; } = new Dictionary<string, int>();
    }
}

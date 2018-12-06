using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.YandexDirect
{
    public class YandexCheckCaptchaQuery : YandexQuery
    {
        public YandexCheckCaptchaQuery(string query, int page, string region, string key, string solution, string retPath) : base(query, page, region)
        {
            Key = key;
            Solution = solution;
            RetPath = retPath;
        }

        public string Key { get; set; }

        public string Solution { get; set; }

        public string RetPath { get; set; }

        public override string Generate(YandexQueryGenerator generator)
        {
            return generator.Generate(this);
        }
    }

}

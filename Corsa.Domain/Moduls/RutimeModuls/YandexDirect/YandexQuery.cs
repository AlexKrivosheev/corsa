using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.YandexDirect
{
    public class YandexQuery
    {
        public YandexQuery(string query, int page, string region)
        {
            Query = query;
            Page = page;
            Region = region;
        }

        public string Query { get; set; }

        public int Page { get; set; }

        public string Region { get; set; }

        public virtual string Generate(YandexQueryGenerator generator)
        {
            return generator.Generate(this);
        }
    }

}

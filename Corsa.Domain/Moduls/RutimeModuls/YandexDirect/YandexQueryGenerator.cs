using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.YandexDirect
{

    public class YandexQueryGenerator
    {
        public string Generate(YandexQuery query)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append($"?text={Uri.EscapeDataString(query.Query)}");

            if (!string.IsNullOrEmpty(query.Region))
            {
                queryBuilder.Append($"&lr={query.Region}");
            }

            if (query.Page > 0)
            {
                queryBuilder.Append($"&p={query.Page}");
            }

            UriBuilder uri = new UriBuilder();
            uri.Host = "www.yandex.ru";
            uri.Scheme = "https";
            uri.Path = "search";
            uri.Query = queryBuilder.ToString();

            return uri.ToString();
        }

        public string Generate(YandexCheckCaptchaQuery query)
        {
            StringBuilder captchaBuilder = new StringBuilder();
            captchaBuilder.Append($"?key={Uri.EscapeDataString(query.Key)}");
            captchaBuilder.Append($"&retpath={Uri.EscapeDataString(query.RetPath)}");
            captchaBuilder.Append($"&rep={Uri.EscapeDataString(query.Solution)}");

            UriBuilder uri = new UriBuilder();
            uri.Host = "www.yandex.ru";
            uri.Scheme = "https";
            uri.Path = "checkcaptcha";
            uri.Query = captchaBuilder.ToString();

            return uri.ToString();
        }

    }
}

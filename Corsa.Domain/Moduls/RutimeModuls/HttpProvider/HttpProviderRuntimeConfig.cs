using System.Net.Http;


namespace Corsa.Domain.Moduls.RutimeModuls.HttpProvider
{
    public class HttpProviderRuntimeConfig
    {
        public bool IsPost { get; set; }

        public HttpContent Content { get; set; }

        public string Query { get; set; }

        public bool AllowAutoRedirect { get; set; } = true;
    }
}

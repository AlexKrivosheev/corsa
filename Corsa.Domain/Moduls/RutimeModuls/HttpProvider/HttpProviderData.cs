using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Corsa.Domain.Moduls.RutimeModuls.HttpProvider
{
    public class HttpProviderData :ModuleData
    {
        public Stream GetContent()
        {
            return Response.Content.ReadAsStreamAsync().Result;
        }

        public HttpResponseMessage Response { get; set; }
    }
}

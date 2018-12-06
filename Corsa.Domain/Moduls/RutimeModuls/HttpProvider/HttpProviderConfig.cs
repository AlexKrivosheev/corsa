using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.HttpProvider
{
    public class ProxySetting
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }
    }

    [Table("HttpProviderConfigs")]
    public class HttpProviderConfig: ModuleСonfig
    {
        public string ProxyHost { get; set; }

        public string ProxyPort { get; set; }

        public string ProxyUser { get; set; }

        public string ProxyPassword { get; set; }      
    }
}

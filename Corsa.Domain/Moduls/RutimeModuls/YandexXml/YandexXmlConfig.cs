using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    [Table("YandexXMLConfigs")]
    public class YandexXmlConfig : ModuleСonfig
    {
        public string User { get; set; }

        public string Key { get; set; }

        public string Region { get; set; }
       
        public string Filter { get; set; }

        public int? PageLimit { get; set; }
        
        public int? HttpModuleId { get; set; }
    }
}

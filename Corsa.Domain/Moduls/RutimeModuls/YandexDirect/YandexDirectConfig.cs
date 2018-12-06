using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Corsa.Domain.Moduls.Providers.YandexXml
{
    [Table("YandexDirectConfigs")]
    public class YandexDirectConfig : ModuleСonfig
    {        
        public string Region { get; set; }
               
        public int? PageLimit { get; set; }
               
        public int? AntigateId { get; set; }

        public int? HttpModuleId { get; set; }
    }
}

using Corsa.Domain.Processing.Moduls;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Moduls.RutimeModuls.HttpProvider
{
    [Table("AntigateConfigs")]
    public class AntigateConfig : ModuleСonfig
    {
        public string ClientKey { get; set; }

        public string LanguagePool { get; set; }

        public int SoftId { get; set; }

        public string CallbackUrl { get; set; }

        public int? HttpModuleId { get; set; }
    }
}

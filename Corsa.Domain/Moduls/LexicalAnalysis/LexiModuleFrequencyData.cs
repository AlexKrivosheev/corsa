using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Processing.Serp;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Moduls.LexicalAnalysis
{
    [Table("LexModuleFrequencyData")]
    public class LexModuleFrequencyData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int LexModuleWordId { get; set; }

        public int LexModuleSerpId { get; set; }

        public int Value { get; set; }

        public int LexModuleStatsId { get; set; }

        [ForeignKey("LexModuleSerpId")]
        public SerpWebPage Page { get; set; }

        [ForeignKey("LexModuleWordId")]
        public RequestWord Word { get; set; }

        [ForeignKey("LexModuleStatsId")]
        public LexModuleStats Stats { get; set; }
    }
}

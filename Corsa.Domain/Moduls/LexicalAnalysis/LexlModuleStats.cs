using Corsa.Domain.Models.Actions;
using Corsa.Domain.Processing.Serp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Moduls.LexicalAnalysis
{    
    [Table("LexModuleStats")]
    public class LexModuleStats
    {
        public int Id { get; set; }        
        public int LexModuleConfigId { get; set; }
        
        public LexModuleStats()
        {
            
        }

        [ForeignKey("LexModuleStatsId")]
        public List<LexModuleFrequencyData> FrequencyData { get; set; } = new List<LexModuleFrequencyData>();

        [ForeignKey("LexModuleStatsId")]
        public List<SerpWebPage> SerpPages { get; set; } = new List<SerpWebPage>();

        [ForeignKey("LexModuleStatsId")]
        public List<RequestWord> Words { get; set; } = new List<RequestWord>();

    }
}

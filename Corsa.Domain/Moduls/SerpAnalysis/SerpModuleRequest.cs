using Corsa.Domain.Models.SearchEngines;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Corsa.Domain.Moduls.SerpAnalysis
{
    [Table("SerpModuleRequests")]
    public class SerpModuleRequest
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }

        public int SerpModuleConfigId { get; set; }
                   
    }
}

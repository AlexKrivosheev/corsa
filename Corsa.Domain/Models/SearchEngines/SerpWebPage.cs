using Corsa.Domain.Moduls.LexicalAnalysis;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Processing.Serp
{
    [Table("LexModuleSerpData")]
    public class SerpWebPage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public int Position { get; set; }

        public byte[] Favicon { get; set; }

        public int TotalWords { get; set; }

        public int LexModuleStatsId { get; set; }

        [ForeignKey("LexModuleStatsId")]
        public LexModuleStats Stats { get; set; }

        public string GetShortUrl()
        {
            if (string.IsNullOrEmpty(Url)){
                return string.Empty;
            }

            try
            {
                var target = new Uri(Url);
                return $"{target.Host}{target.LocalPath}".TrimEnd('/').ToLower();
            }
            catch (Exception exc)
            {

            }

            return Url;
        }
    }
}

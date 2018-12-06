using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Moduls.LexicalAnalysis
{
    [Table("LexModuleWordData")]
    public class RequestWord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Details { get; set; }

        public int MedianValue{ get; set; }

        public int LexModuleStatsId { get; set; }

        [ForeignKey("LexModuleStatsId")]
        public LexModuleStats Stats { get; set; }
        
    }
}

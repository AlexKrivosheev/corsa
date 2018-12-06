using Corsa.Domain.Models.Actions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.Projects
{
    [Table("ProjectModuleResults")]
    public class ProjectModuleResult
    {
        public int Id { get; set; }
        public int ProjectModuleId { get; set; }
        public int UserActionStatsId { get; set; }

        [ForeignKey("UserActionStatsId")]
        public UserActionDetails Stats { get; set; }

        [ForeignKey("ProjectModuleId")]
        public ProjectModule Modul { get; set; }

    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.Projects
{
    public class LinkedModule
    {
        public int Id { get; set; }

        public int ProjectModuleId { get; set; }

        public int LinkedProjectModuleId { get; set; }

        public string Description { get; set; }
        
        [ForeignKey("LinkedProjectModuleId")]
        public ProjectModule LinkedProjectModule { get; set; }

    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.Projects
{
    [Table("Projects")]
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }

        [ForeignKey("ProjectId")]
        public List<ProjectModule> Moduls { get; set; } = new List<ProjectModule>();
    }
}

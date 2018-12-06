using Corsa.Domain.Models.Projects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Processing.Moduls
{
    public class ModuleСonfig
    {
        public int Id { get; set; }

        public int ProjectModuleId { get; set; }

        [ForeignKey("ProjectModuleId")]
        public ProjectModule ProjectModule { get; set; } = new ProjectModule();
    }
}

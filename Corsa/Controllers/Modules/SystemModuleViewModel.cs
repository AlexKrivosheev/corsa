using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.SystemConfig;

namespace Corsa.Models.Moduls
{
    public class SystemModuleViewModel
    {
        public SystemModuleViewModel(Project project, SystemModule module, ProjectModuleViewDescriptor viewDescriptor)
        {
            this.Module = module;
            this.Project = project;
            this.ViewDescriptor = viewDescriptor;
        }

        public SystemModule Module { get; set; }

        public Project Project { get; set; }

        public ProjectModuleViewDescriptor ViewDescriptor { get; set; }

    }
}

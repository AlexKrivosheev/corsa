using Corsa.Domain.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Corsa.Models.Moduls
{
    public class ProjectModuleViewModel
    {
        public ProjectModuleViewModel(Project project, ProjectModule module, ProjectModuleViewDescriptor viewDescriptor)
        {
            this.Module = module;
            this.Project = project;
            this.ViewDescriptor = viewDescriptor;
        }

        public ProjectModule Module { get; set; }

        public Project Project { get; set; }

        public ProjectModuleViewDescriptor ViewDescriptor { get; set; }

    }
}

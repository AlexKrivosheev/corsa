using Corsa.Domain.Models.Projects;
using Corsa.Models.Moduls;
using System.Collections.Generic;
using System.Linq;

namespace Corsa.Models.Config
{
    public class ConfigViewModel
    {
        public ConfigViewModel(Project project, ProjectModuleViewRegistry modulViewRegistry)
        {
            this.Instance = project;
            this.ModulViewRegistry = modulViewRegistry;
        }

        public Project Instance { get; private set; }

        public ProjectModuleViewRegistry ModulViewRegistry;

        public List<ProjectModuleViewDescriptor> RegisteredModules
        {
            get
            {
                return ModulViewRegistry.GetModuls().Where(module=>module.IsSystem).ToList();
            }
        }
    }
}

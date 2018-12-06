using Corsa.Domain.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Corsa.Models.Modules
{
    public class LinkedProjectModuleViewModel
    {
        public LinkedProjectModuleViewModel(List<ModuleViewModel> availableModules, List<LinkedModule> linkedModules)
        {
            AvailableModules = availableModules;
            LinkedModules = linkedModules;
        }

        public List<ModuleViewModel> AvailableModules { get; set; }

        public List<LinkedModule>LinkedModules { get; set; }
    }
}

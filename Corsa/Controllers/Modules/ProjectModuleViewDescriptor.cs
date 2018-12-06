using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Corsa.Models.Moduls
{
    public class ProjectModuleViewDescriptor
    {
        public ProjectModuleViewDescriptor(string name, string description, string controller, string creatAction, string editAction, string openAction,string projectShowcase, string toolboxShowcase, bool isSystem)
        {
            this.Name = name;
            this.Controller = controller;
            this.CreateAction = creatAction;
            this.EditAction = editAction;
            this.OpenAction = openAction;
            this.Description = description;
            this.ProjectShowCase = projectShowcase;
            this.ToolboxShowCase = toolboxShowcase;
            this.IsSystem = isSystem;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Controller { get; set; }

        public string EditAction { get; set; }

        public string OpenAction { get; set; }

        public string CreateAction { get; set; }

        public string ProjectShowCase { get; set; }

        public string ToolboxShowCase { get; set; }

        public bool IsSystem { get; set; }

    }
}

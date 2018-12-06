using Corsa.Domain.Exceptions;
using Corsa.Models.Moduls;
using System.Collections.Generic;
using System.Linq;

namespace Corsa.Models
{
    public class ProjectModuleViewRegistry
    {
        public Dictionary<int, ProjectModuleViewDescriptor> _registry = new Dictionary<int, ProjectModuleViewDescriptor>();
    
        public void Register(int code, ProjectModuleViewDescriptor descriptor)
        {
            if (_registry.ContainsKey(code))
            {
                throw new UserException($"Modul {code} has registered allready");
            }

            _registry.Add(code, descriptor);
        }

        public ProjectModuleViewDescriptor Find(int code)
        {
            ProjectModuleViewDescriptor descriptor;
            if (!_registry.TryGetValue(code, out descriptor))
            {
                throw new UserException($"Modul {code} isn't find");
            }

            return descriptor;
        }

        public List<ProjectModuleViewDescriptor> GetModuls()
        {
            return _registry.Values.ToList();
        }

        public ProjectModuleViewDescriptor this[int code]
        {

            get
            {
                return _registry[code];
            }
        }
    }
}

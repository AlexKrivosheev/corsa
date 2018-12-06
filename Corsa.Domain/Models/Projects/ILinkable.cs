using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Models.Projects
{
    public interface ILinkable
    {
        ICollection<LinkedModule> GetLinkedModules();
    }
}

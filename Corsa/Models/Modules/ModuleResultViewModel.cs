using Corsa.Domain.Moduls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Corsa.Models.Moduls
{
    public class ModuleResultViewModel<TData>
    {
        public ModuleResultViewModel(int moduleId, ModuleTaskResult<TData> result)
        {
            this.ModuleId = moduleId;
            this.Result = result;
        }

        public ModuleTaskResult<TData>  Result { get; set; }

        public int ModuleId { get; set; }

    }
}

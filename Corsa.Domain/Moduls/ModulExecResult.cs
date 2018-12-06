using Corsa.Domain.Models.Actions;
using System;

namespace Corsa.Domain.Moduls
{
    public class ModuleTaskResult<TData> : ModuleResult
    {

        public UserActionDetails Details { get; set; }

        public TData Data { get; set; }

        public Exception Error { get; set; }

        public bool IsSuccessfully
        {
            get { return Error == null; }
        }
    }
}

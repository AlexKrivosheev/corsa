using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Actions;
using System;

namespace Corsa.Domain.Infrastructure.Exceptions
{
    public class RuntimeModuleException : UserException
    {
        public string ModuleName { get; set; }

        public UserActionDetails Details { get; set; }

        public RuntimeModuleException(string moduleName,string message, Exception innerException, UserActionDetails details) : base(message, innerException)
        {
            this.ModuleName = moduleName;
            this.Details = details;
        }
    }
}

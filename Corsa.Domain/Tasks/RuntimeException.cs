using Corsa.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Tasks
{
    public class RuntimeException : UserException
    {

        public string ModuleName { get; set; }

        public RuntimeException(string message, string moduleName, Exception exception) : base(message, exception)
        {            
            this.ModuleName = moduleName;
        }
    }
}

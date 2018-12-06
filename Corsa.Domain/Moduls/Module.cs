using Corsa.Domain.Exceptions;
using Corsa.Domain.Infrastructure.Exceptions;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.Requests;
using System;

namespace Corsa.Domain.Processing.Moduls
{
    public interface IModule
    {
        int Id { get; set; }

        int Code { get; }

        string Name { get; }

        RuntimeContext Context { get; set; }
    }

    public abstract class Module: IModule
    {
        public int Id { get; set; }

        public abstract int Code { get;  }

        public abstract string Name { get; }

        public RuntimeContext Context { get;set; }        
    }
}

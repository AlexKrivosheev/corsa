using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls
{
    public interface IRuntimeModule<TConfig, TData> : IModule
    {
        TData Run(TConfig config);
    }

    public interface IRuntimeModule<TData> : IModule
    {
        TData Run();
    }
}

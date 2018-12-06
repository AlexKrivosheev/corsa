using Corsa.Domain.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Corsa.Infrastructure.Logging
{
    public class AppLogProvider : ILoggerProvider
    {
        System.IServiceProvider Provider { get; set; }

        private readonly Func<string, LogLevel, bool> _filter;
        
        public AppLogProvider(System.IServiceProvider provider, Func<string, LogLevel, bool> filter = null)
        {
            _filter = filter;
            this.Provider = provider;            
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            var repository = this.Provider.GetService<ILogReprository>();

            return new AppLogger(categoryName,_filter, repository);
        }

        public void Dispose()
        {
            
        }
    }
}

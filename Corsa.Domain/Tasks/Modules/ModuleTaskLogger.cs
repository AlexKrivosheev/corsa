using Corsa.Domain.Processing;
using Microsoft.Extensions.Logging;
using System;


namespace Corsa.Domain.Tasks.Modules
{
    public class ModuleTaskLogger : ILogger
    {
        ILogger Logger { get; set; }
        ITask Task { get; set; }

        public ModuleTaskLogger(ITask Task, ILogger logger)
        {
            this.Logger = logger;
            this.Task = Task;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return Logger.BeginScope<TState>(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var userLogState = state as UserLogState;
                if (userLogState != null)
                {
                    Task.State.Message = userLogState.Message;
                }
            }

            Logger.Log<TState>(logLevel, eventId, state, exception, formatter);
        }
    }
}

using Corsa.Domain.Logging;
using Corsa.Domain.Processing;
using Microsoft.Extensions.Logging;
using System;

namespace Corsa.Infrastructure.Logging
{
    public class AppLogger : ILogger
    {
        public static string UserCategory = "User";
        public static string ProjectCategory = "Project";
        public static string SystemCategory = "System";

        private readonly Func<string, LogLevel, bool> _filter;
        private string _category;
        
        public AppLogger(string category, Func<string, LogLevel, bool> filter, ILogReprository repository)
        {
            _filter = filter;
            _repository = repository;
            _category = category;
        }

        private ILogReprository _repository;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter == null || _filter(_category, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {            
            if (IsEnabled(logLevel))
            {
                var userLogState = state as UserLogState;
                if (userLogState != null)
                {
                    var result = formatter(state, exception);

                    var log = new Log()
                    {
                        LogLevel = logLevel.ToString(),
                        ActionId = eventId.Id,
                        Message = userLogState.Message ?? exception?.Message,
                        CreatedTime = DateTime.Now,
                        UserId = userLogState.Context?.User,
                        UserActionStatsId = userLogState.ActionStatisticId
                    };

                    _repository.AddLog(log);
                }
            }
        }
    }
}

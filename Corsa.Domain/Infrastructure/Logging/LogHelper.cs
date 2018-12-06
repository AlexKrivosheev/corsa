using Corsa.Domain.Processing;
using Microsoft.Extensions.Logging;
using System;

namespace Corsa.Domain.Logging
{
    public static class LogHelper
    {
        public static string Format(UserLogState state, Exception exception)
        {
            return string.Empty;
        }

        public static void Inform(this ILogger logger, int action, string message, RuntimeContext runtimeContext, Exception exception = null) {

            LogWrite(logger, LogLevel.Information, action, new UserLogState() { Message = message, Context = runtimeContext}, exception);
        }

        public static void Error(this ILogger logger, int action, string message, RuntimeContext runtimeContext, Exception exception = null)
        {
            LogWrite(logger, LogLevel.Error, action, new UserLogState() { Message = message, Context = runtimeContext}, exception);
        }

        public static void Warning(this ILogger logger, int action, string message, RuntimeContext runtimeContext, Exception exception = null)
        {
            LogWrite(logger, LogLevel.Warning, action, new UserLogState() { Message = message, Context = runtimeContext }, exception);
        }

        public static void LogWrite(ILogger logger, LogLevel level, int action, UserLogState state, Exception exception)
        {
            logger.Log<UserLogState>(level, action, state, exception, Format);
        }
    }
}

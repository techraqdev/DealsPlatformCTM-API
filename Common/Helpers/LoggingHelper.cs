using Microsoft.Extensions.Logging;
using System.Web;

namespace Common.Helpers
{
    public static class LoggingHelper
    {
        public static void Log(ILogger logger, LogLevel logLevel, Exception? exception, string? message, params object?[] args)
        {
            logger.Log(logLevel, exception, message, args);
        }


        public static void Log(ILogger logger, LogLevel logLevel, string? message, params object?[] args)
        {
            logger.Log(logLevel,HttpUtility.UrlEncode(message), args);
        }

    }
}

using Contracts;
using Microsoft.Extensions.Logging;
using System;
namespace LoggerManager
{
    public class LoggerManager<T> : ILoggerManager<T>  where T:class 
    {
        private readonly ILogger<T> logger;

        public LoggerManager(ILogger<T> logger)
        {
            this.logger = logger;// Serilog.Log.Logger;
        }
        public void LogDebug(string message)
        {
            logger.LogDebug(message);
        }

        public void LogError(string message)
        {
            logger.LogError(message);
        }

        public void LogInfo(string message)
        {
            logger.LogInformation(message);
        }

        public void LogWarn(string message)
        {
            logger.LogWarning(message);
        }
    }

    public class LoggerManager: ILoggerManager
    {
        private readonly Serilog.ILogger logger;

        public LoggerManager(Serilog.ILogger  logger)
        {
            this.logger = logger;
        }
        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogInfo(string message)
        {
            logger.Information(message);
        }

        public void LogWarn(string message)
        {
            logger.Warning(message);
        }
    }
}

using System;

namespace Contracts
{
    public interface ILoggerManager<T>: ILoggerManager where T :class
    {
    }

    public interface ILoggerManager 
    {
        public void LogInfo(string message);
        public void LogWarn(string message);
        public void LogDebug(string message);
        public void LogError(string message);
    }
}

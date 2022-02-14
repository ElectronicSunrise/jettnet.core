using System;
using System.Diagnostics;

namespace jettnet.core
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public class Logger
    {
        private readonly Action<object> _error   = Console.Error.WriteLine;
        private readonly Action<object> _info    = Console.WriteLine;
        private readonly Action<object> _warning = Console.WriteLine;

        private readonly string _prefix;

        private readonly object _lock = new object();

        public Logger(string prefix = "JETTNET")
        {
            _prefix = $"[{prefix}]";
        }

        public Logger(Action<object> info, Action<object> warning, Action<object> error, string prefix = "JETTNET")
        {
            _info    = info;
            _warning = warning;
            _error   = error;
            _prefix  = $"[{prefix}]";
        }

        public void Log(object msg, LogLevel logLevel = LogLevel.Info)
        {
            string message = $"{_prefix} {msg}";

            lock (_lock)
            {
                switch (logLevel)
                {
                    case LogLevel.Info:
                        _info?.Invoke(message);
                        break;
                    case LogLevel.Warning:
                        _warning?.Invoke(message);
                        break;
                    case LogLevel.Error:
                        _error?.Invoke(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
                }
            }
        }

        [Conditional("DEBUG")]
        public void LogDebug(object msg, LogLevel logLevel = LogLevel.Info) 
        {
            Log(msg, logLevel);
        }
    }
}
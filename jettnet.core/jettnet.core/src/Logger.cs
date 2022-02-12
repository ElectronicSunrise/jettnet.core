using System;

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
            
#if UNITY_64
            switch (logLevel)
            {
                case LogLevel.Info:
                    UnityEngine.Debug.Log(message);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
            }
            
            return;
#else
            switch (logLevel)
            {
                case LogLevel.Info:
                    
                    lock (_info)
                        _info?.Invoke(message);
                    
                    break;
                case LogLevel.Warning:
                    
                    lock (_warning)
                        _warning?.Invoke(message);
                    
                    break;
                case LogLevel.Error:
                    
                    lock (_error)
                        _error?.Invoke(message);
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
#endif
        }
    }
}

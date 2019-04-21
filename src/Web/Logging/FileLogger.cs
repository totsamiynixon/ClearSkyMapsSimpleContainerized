using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _errorPath;
        private readonly string _infoPath;
        private object _lock = new object();
        public FileLogger(string errorPath, string infoPath)
        {
            _errorPath = errorPath;
            _infoPath = infoPath;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
                    {
                        File.AppendAllText(_errorPath, formatter(state, exception) + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(_infoPath, formatter(state, exception) + Environment.NewLine);
                    }
                }
            }
        }
    }
}

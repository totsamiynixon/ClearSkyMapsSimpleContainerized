using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _errorPath;
        private readonly string _infoPath;
        public FileLoggerProvider(string errorPath, string infoPath)
        {
            _errorPath = errorPath;
            _infoPath = infoPath;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_errorPath, _infoPath);
        }

        public void Dispose()
        {
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Logging
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory,
                                        string errorPath, string infoPath)
        {
            factory.AddProvider(new FileLoggerProvider(errorPath, infoPath));
            return factory;
        }
    }
}

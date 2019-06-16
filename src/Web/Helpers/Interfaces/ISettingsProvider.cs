using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Helpers.Interfaces
{
    public interface ISettingsProvider
    {
        bool IsDevelopment { get; }

        bool IsStaging { get; }

        bool IsProduction { get; }

        bool EmulationEnabled { get; }

        string FirebaseCloudMessagingServerKey { get; }

        string FirebaseCloudMessagingMessagingSenderId { get; }

        string ApplicationVersion { get; }

        string ApplicationEnvironment { get; }

        string ConnectionString { get; }

        string IdentityConnectionString { get; }

        string YandexMapsJavaScriptAPIKey { get; }
    }
}

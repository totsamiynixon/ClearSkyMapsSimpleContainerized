using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Admin.Emulation;
using Web.Helpers.Interfaces;

namespace Web.Helpers.Implementations
{
    public class JsonConfigSettingsProvider : ISettingsProvider
    {
        private static JObject Settings { get; set; }
        private static readonly object Lock = new object();
        private static bool _configInitialized;

        private static readonly List<string> requiredSettings = new List<string>()
        {
            "FirebaseCloudMessaging:ServerKey",
            "FirebaseCloudMessaging:MessagingSenderId",
            "Application:Version",
            "Application:Environment",
            "ConnectionString",
            "Emulation:ConnectionString",
            "Emulation:Enabled",
            "Yandex:MapsJavaScriptAPIKey",
            "Server:IP"
        };

        private readonly IHostingEnvironment _env;
        public JsonConfigSettingsProvider(IHostingEnvironment env)
        {
            _env = env;
            if (_configInitialized)
            {
                return;
            }
            lock (Lock)
            {
                if (!_configInitialized)
                {
                    InitConfig();
                    _configInitialized = true;
                }
            }
        }


        private void InitConfig()
        {
            using (StreamReader streamReader = new StreamReader(Path.Combine(_env.ContentRootPath, "config.json"), Encoding.UTF8))
            {
                var jsonString = streamReader.ReadToEnd();
                Settings = JsonConvert.DeserializeObject<JObject>(jsonString);
                if (IsDevelopment)
                {
                    Settings["Application:Version"] = $"{DateTime.UtcNow.Day}.{DateTime.UtcNow.Millisecond}";
                }
                var notprovidedKeys = requiredSettings.Except(requiredSettings.Intersect(Settings.Properties().Select(f => f.Name))).ToList();
                if (notprovidedKeys.Any())
                {
                    throw new KeyNotFoundException($"No such keys:[{string.Join(", ", notprovidedKeys)}] in config.json");
                }
            }
        }


        public bool IsDevelopment => ApplicationEnvironment == "Development";

        public bool IsStaging => ApplicationEnvironment == "Staging";

        public bool IsProduction => ApplicationEnvironment == "Production";

        public bool EmulationEnabled => bool.Parse(Settings["Emulation:Enabled"].ToString());

        public string FirebaseCloudMessagingServerKey => Settings["FirebaseCloudMessaging:ServerKey"].ToString();

        public string FirebaseCloudMessagingMessagingSenderId => Settings["FirebaseCloudMessaging:MessagingSenderId"].ToString();

        public string ApplicationVersion => Settings["Application:Version"].ToString();

        public string ApplicationEnvironment => Settings["Application:Environment"].ToString();

        public string ConnectionString => Emulator.IsEmulationEnabled ? Settings["Emulation:ConnectionString"].ToString() : Settings["ConnectionString"].ToString();

        public string IdentityConnectionString => Settings["ConnectionString"].ToString();

        public string YandexMapsJavaScriptAPIKey => Settings["Yandex:MapsJavaScriptAPIKey"].ToString();

        public string ServerIP => Settings["Server:IP"].ToString();
    }
}

using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Helpers.Interfaces;

namespace Web.Areas.PWA.Helpers.Implementations
{
    public class PWAFileCompilerBootstrapper : IPWABootstrapper
    {
        private static readonly object Lock = new object();
        private static bool _compiled;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ISettingsProvider _settingsProvider;
        public PWAFileCompilerBootstrapper(IHostingEnvironment hostingEnvironment, ISettingsProvider settingsProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _settingsProvider = settingsProvider;
        }

        public void InitializePWA()
        {
            if (!_compiled)
            {
                lock (Lock)
                {
                    CompileManifest();
                    CompileServiceWorker();
                    _compiled = true;
                }
            }
        }

        private void CompileManifest()
        {
            JObject manifest = null;
            var manifestPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Areas/PWA/manifest.json");
            var manifestWritePath = Path.Combine(_hostingEnvironment.WebRootPath, "pwa/manifest.json");
            using (var reader = File.OpenText(manifestPath))
            {
                var jsonString = reader.ReadToEnd();
                manifest = JsonConvert.DeserializeObject<JObject>(jsonString);
                if (!_settingsProvider.IsProduction)
                {
                    manifest["name"] = manifest["name"] + " " + _settingsProvider.ApplicationEnvironment;
                    manifest["short_name"] = manifest["short_name"] + " " + _settingsProvider.ApplicationEnvironment;
                }
                manifest["gsm_sender_id"] = _settingsProvider.FirebaseCloudMessagingMessagingSenderId;
            }
            File.WriteAllText(manifestWritePath, JsonConvert.SerializeObject(manifest));
        }


        private void CompileServiceWorker()
        {
            string jsString;
            var serviceWorkerPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Areas/PWA/service-worker.js");
            var serviceWorkerWritePath = Path.Combine(_hostingEnvironment.WebRootPath, "pwa/service-worker.js");
            using (var reader = File.OpenText(serviceWorkerPath))
            {
                jsString = reader.ReadToEnd();
                jsString = jsString.Replace("%MessagingSenderId%", _settingsProvider.FirebaseCloudMessagingMessagingSenderId);
                jsString = jsString.Replace("%Version%", _settingsProvider.ApplicationVersion);
            }
            File.WriteAllText(serviceWorkerWritePath, jsString);
        }
    }
}

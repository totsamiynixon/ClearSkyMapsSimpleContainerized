namespace Web.Areas.PWA.Infrastructure
{
    public class PWAAppSettings
    {
        public YandexAppSettings Yandex { get; set; }
    }
    
    public class YandexAppSettings
    {
        public string MapsJavaScriptAPIKey { get; set; }
    }
}
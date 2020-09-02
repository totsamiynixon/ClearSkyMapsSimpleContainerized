namespace Web.Infrastructure.AntiForgery
{
    public class AntiForgerySettings
    {
        public static string AntiForgeryFieldName { get; } = "__CSMAntiForgery";
        public static string AntiForgeryCookieName { get; } = "CSMAntiForgeryCookie";
    }
}
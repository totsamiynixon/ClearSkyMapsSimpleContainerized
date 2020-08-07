using Microsoft.AspNetCore.Builder;

namespace Web.Areas.PWA.Infrastructure.MIddlewares
{
    public static class BundleConfigurationMiddleware
    {
        public static IApplicationBuilder AppBundles(this IApplicationBuilder builder)
        {
            builder.UseBundling(
                bundles =>
                {
                    bundles.AddJs("/pwa.js")
                        .Include("/pwa/js/libs/jquery-3.3.1.min.js")
                        .Include("/pwa/js/libs/popper.js")
                        .Include("/pwa/js/libs/bootstrap-material-design.js")
                        .Include("/pwa/js/libs/Chart.bundle.min.js")
                        .Include("/pwa/js/libs/moment.js")
                        .Include("/pwa/js/libs/vue.js")
                        .Include("/pwa/js/libs/vue-router.js")
                        .Include("/pwa/js/libs/signalr.min.js")
                        .Include("/pwa/js/front/main.js")
                        .Include("/pwa/js/front/vue-filters.js")
                        .Include("/pwa/js/front/readings.js")
                        .Include("/pwa/js/front/offline.js")
                        .Include("/pwa/js/front/app.js");

                    bundles.AddCss("/pwa.css")
                        .Include("/pwa/css/libs/bootstrap-material-design.css")
                        .Include("/pwa/css/front/site.css");
                    
                });
            return builder;
        }
    }
}

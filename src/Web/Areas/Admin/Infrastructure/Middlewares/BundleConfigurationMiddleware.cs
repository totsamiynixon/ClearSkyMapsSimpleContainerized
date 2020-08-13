using Microsoft.AspNetCore.Builder;

namespace Web.Areas.Admin.Infrastructure.Middlewares
{
    public static class BundleConfigurationMiddleware
    {
        public static IApplicationBuilder AppBundles(this IApplicationBuilder builder)
        {
            builder.UseBundling(
                bundles =>
                {
                    bundles.AddJs("/admin.js")
                    .Include("/admin/theme/js/core/jquery.min.js")
                    .Include("/admin/theme/js/core/popper.min.js")
                    .Include("/admin/theme/js/core/bootstrap.min.js")
                    .Include("/admin/theme/js/plugins/perfect-scrollbar.jquery.min.js")
                    .Include("/admin/theme/js/plugins/chartjs.min.js")
                    .Include("/admin/theme/js/plugins/bootstrap-notify.js")
                    .Include("/admin/theme/js/paper-dashboard.min.js")
                    .Include("/admin/js/libs/signalr.min.js")
                    .Include("/admin/js/libs/moment.js")
                    .Include("/admin/js/libs/vue.js")
                    .Include("/admin/js/front/main.js")
                    .Include("/admin/js/front/pages/sensors.js");

                    bundles.AddCss("/admin.css")
                        .Include("/admin/theme/css/bootstrap.min.css")
                        .Include("/admin/theme/css/paper-dashboard.min.css")
                        .Include("/admin/css/site.css");
                });
            return builder;
        }
    }
}

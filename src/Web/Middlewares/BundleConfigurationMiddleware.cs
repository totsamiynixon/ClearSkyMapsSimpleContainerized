using Karambolo.AspNetCore.Bundling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web.Middlewares
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


    //private class CssRewriteUrlTransformFixed : IBundleItemTransform
    //{
    //    private static string RebaseUrlToAbsolute(string baseUrl, string url, string prefix, string suffix)
    //    {
    //        if (string.IsNullOrWhiteSpace(url)
    //            || string.IsNullOrWhiteSpace(baseUrl)
    //            || url.StartsWith("/", StringComparison.OrdinalIgnoreCase)
    //            || url.StartsWith("http://") || url.StartsWith("https://"))
    //        {
    //            return url;
    //        }

    //        if (url.StartsWith("data:"))
    //        {
    //            // Keep the prefix and suffix quotation chars as is in case they are needed (e.g. non-base64 encoded svg)
    //            return prefix + url + suffix;
    //        }

    //        if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
    //        {
    //            baseUrl += "/";
    //        }

    //        return $"'{VirtualPathUtility.ToAbsolute(baseUrl + url)}'";
    //    }

    //    private static string ConvertUrlsToAbsolute(string baseUrl, string content)
    //    {
    //        if (string.IsNullOrWhiteSpace(content))
    //        {
    //            return content;
    //        }

    //        var regex = new Regex("url\\((?<prefix>['\"]?)(?<url>[^)]+?)(?<suffix>['\"]?)\\)");

    //        return regex.Replace(content, match => "url(" + RebaseUrlToAbsolute(baseUrl, match.Groups["url"].Value, match.Groups["prefix"].Value, match.Groups["suffix"].Value) + ")");
    //    }

    //    public void Transform(IBundleItemTransformContext context)
    //    {
    //        if (!string.IsNullOrEmpty(context.Content))
    //        {
    //            return ConvertUrlsToAbsolute(context.BuildContext.Bundle., input);
    //        }
    //    }
    //}
}

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ToJson<T>(this IHtmlHelper helper, T model)
        {
            return helper.Raw(JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }
    }
}

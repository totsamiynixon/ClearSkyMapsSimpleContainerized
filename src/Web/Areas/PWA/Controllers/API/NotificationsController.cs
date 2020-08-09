using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.PWA.Models.Notifications;
using Web.Infrastructure;

namespace Web.Areas.PWA.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        public NotificationsController( AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeOnSensorAsync(SubscribeOnSersorModel model)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + _appSettings.FirebaseCloudMessaging.ServerKey);
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://iid.googleapis.com/iid/v1/{model.RegistrationToken}/rel/topics/sensor_{model.SensorId}");
            request.Content = new StringContent("",
                Encoding.UTF8,
                "application/json");
            var response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> UnsubscribeFromSensorAsync(SubscribeOnSersorModel model)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + _appSettings.FirebaseCloudMessaging.ServerKey);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://iid.googleapis.com/iid/v1/{model.RegistrationToken}/rel/topics/sensor_{model.SensorId}");
            request.Content = new StringContent("",
                Encoding.UTF8,
                "application/json");
            var response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
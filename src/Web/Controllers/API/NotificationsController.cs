using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models.API.Notifications;

namespace Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public NotificationsController(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        /// <summary>
        /// Subscribes client on notifications for sensors
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">If successfully subscribed</response>
        /// <response code="400">If failed to subscribe</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubscribeOnSensorAsync(SubscribeOnSersorModel model)
        {
            return await CallFirebaseApiAsync(model, HttpMethod.Post);
        }

        /// <summary>
        /// Unsubscribe client from sensor notifications
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">If successfully unsubscribed</response>
        /// <response code="400">If failed to unsubscribe</response>  
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnsubscribeFromSensorAsync(SubscribeOnSersorModel model)
        {
            return await CallFirebaseApiAsync(model, HttpMethod.Delete);
        }

        private async Task<IActionResult> CallFirebaseApiAsync(SubscribeOnSersorModel model, HttpMethod method)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("key", "=" + _appSettings.FirebaseCloudMessaging.ServerKey);
            var request = new HttpRequestMessage(method,
                $"https://iid.googleapis.com/iid/v1/{model.RegistrationToken}/rel/topics/sensor_{model.SensorId}");
            request.Content = new StringContent("",
                Encoding.UTF8,
                "application/json");
            var response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
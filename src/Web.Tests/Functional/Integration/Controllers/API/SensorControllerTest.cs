using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Web.Tests.Functional.Integration.Controllers.API
{
    public class SensorControllerTest : BaseScenario
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public SensorControllerTest()
        {
            // Arrange
            _server = CreateServer();
            _client = _server.CreateClient();
        }

        
        [Fact]
        public async Task Get_get_all_sensors_and_response_ok_status_code()
        {
            //Act
            var response = await _client.GetAsync("api/sensors");

            //Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
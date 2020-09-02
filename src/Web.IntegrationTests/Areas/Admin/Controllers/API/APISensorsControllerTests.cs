using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.Default.Sensors;
using Web.Domain.Entities;
using Web.Helpers;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Web.IntegrationTests.Infrastructure;
using Xunit;

namespace Web.IntegrationTests.Areas.Admin.Controllers.API
{
    [Collection("Integration: Sequential")]
    public class APISensorsControllerTests : BaseScenario
    {
        [Fact]
        public async Task Get_get_all_sensors_and_response_ok_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync("api/admin/sensors");

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_static_sensor_and_response_ok_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var staticSensor = new CreateStaticSensorModel
                {ApiKey = CryptoHelper.GenerateApiKey(), Latitude = Defaults.Latitude, Longitude = Defaults.Longitude};
            var staticSensorJsonStr = JsonConvert.SerializeObject(staticSensor);

            //Act
            var response = await client.PostAsync("api/admin/sensors/static",
                new StringContent(staticSensorJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task Create_static_sensor_and_response_bad_request_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/sensors/static",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_portable_sensor_and_response_ok_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var portableSensor = new CreatePortableSensorModel {ApiKey = CryptoHelper.GenerateApiKey()};
            var portableSensorJsonStr = JsonConvert.SerializeObject(portableSensor);

            //Act
            var response = await client.PostAsync("api/admin/sensors/portable",
                new StringContent(portableSensorJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task Create_portable_sensor_and_response_bad_request_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/sensors/portable",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(0, true)]
        [InlineData(1, true)]
        public async Task Delete_sensor_and_response_ok_status_code(int dataSetIndex, bool isCompletely)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.PortableSensor,
                Defaults.StaticSensor
            };
            var currentSensor = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseSensors(currentSensor)
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var deleteSensor = new DeleteSensorModel {Id = currentSensor.Id, IsCompletely = isCompletely};
            var deleteSensorJsonStr = JsonConvert.SerializeObject(deleteSensor);

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/sensors")
            {
                Content = new StringContent(deleteSensorJsonStr, Encoding.UTF8, "application/json")
            });

            //Assert
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task Delete_sensor_and_response_forbidden_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/sensors")
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Delete_sensor_and_response_bad_request_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/sensors")
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_sensor_and_response_not_found_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var deleteSensor = new DeleteSensorModel {Id = 1};
            var deleteSensorJsonStr = JsonConvert.SerializeObject(deleteSensor);

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/sensors")
            {
                Content = new StringContent(deleteSensorJsonStr, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(0, true)]
        [InlineData(1, true)]
        public async Task Change_sensor_activation_and_response_ok_status_code(int dataSetIndex, bool isActive)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.ActivePortableSensor,
                Defaults.ActiveStaticSensor
            };
            var currentSensor = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseSensors(currentSensor)
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var changeActivationSensor = new ChangeActivationSensorModel() {Id = currentSensor.Id, IsActive = isActive};
            var changeActivationSensorJsonStr = JsonConvert.SerializeObject(changeActivationSensor);

            //Act
            var response = await client.PostAsync("api/admin/sensors/changeActivation",
                new StringContent(changeActivationSensorJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task Change_sensor_activation_and_response_bad_request_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/sensors/changeActivation",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Change_sensor_activation_and_response_not_found_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var changeActivationSensor = new ChangeActivationSensorModel {Id = 1};
            var changeActivationSensorJsonStr = JsonConvert.SerializeObject(changeActivationSensor);

            //Act
            var response = await client.PostAsync("api/admin/sensors/changeActivation",
                new StringContent(changeActivationSensorJsonStr, Encoding.UTF8, "application/json"));
            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Change_static_sensor_visibility_and_response_ok_status_code(bool isVisible)
        {
            //Arrange
            var currentSensor = Defaults.ActiveStaticSensor;

            using var server = new TestServerBuilder()
                .UseSensors(currentSensor)
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var changeVisibilitySensor = new ChangeVisibilityStaticSensorModel
                {Id = currentSensor.Id, IsVisible = isVisible};
            var changeVisibilitySensorJsonStr = JsonConvert.SerializeObject(changeVisibilitySensor);

            //Act
            var response = await client.PostAsync("api/admin/sensors/static/changeVisibility",
                new StringContent(changeVisibilitySensorJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task Change_static_sensor_visibility_and_response_bad_request_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/sensors/static/changeVisibility",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Change_static_sensor_visibility_and_response_not_found_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var changeVisibilityStaticSensor = new ChangeVisibilityStaticSensorModel {Id = 1};
            var changeVisibilityStaticSensorJsonStr = JsonConvert.SerializeObject(changeVisibilityStaticSensor);

            //Act
            var response = await client.PostAsync("api/admin/sensors/changeActivation",
                new StringContent(changeVisibilityStaticSensorJsonStr, Encoding.UTF8, "application/json"));
            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
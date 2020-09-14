using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using NUglify.Helpers;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.Default.Sensors;
using Web.Domain.Entities;
using Web.Helpers;
using Web.IntegrationTests.Areas.Admin.Extensions;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Web.IntegrationTests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Web.IntegrationTests.Areas.Admin.Controllers.Default
{
    [Collection("Integration: Sequential")]
    public class SensorsControllerTests : BaseScenario
    {
        private readonly ITestOutputHelper _outputHelper;

        public SensorsControllerTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }


        [Theory]
        [InlineData("admin")]
        [InlineData("admin/sensors")]
        [InlineData("admin/sensors/index")]
        public async Task Get_get_all_sensors_and_response_ok_status_code_with_correct_content_type(string url)
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync(url);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Get_create_static_sensor_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync("admin/sensors/createStaticSensor");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_create_static_sensor_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("admin/sensors/createStaticSensor", new StringContent(string.Empty));

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_create_static_sensor_and_response_redirect_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var payloadDict = new Dictionary<string, string>
            {
                {
                    $"{nameof(CreateStaticSensorViewModel.Model)}.{nameof(CreateStaticSensorModel.Latitude)}",
                    Defaults.Latitude.ToCommaSeparatedString()
                },
                {
                    $"{nameof(CreateStaticSensorViewModel.Model)}.{nameof(CreateStaticSensorModel.Longitude)}",
                    Defaults.Longitude.ToCommaSeparatedString()
                },
                {
                    $"{nameof(CreateStaticSensorViewModel.Model)}.{nameof(CreateStaticSensorModel.ApiKey)}",
                    (CryptoHelper.GenerateApiKey())
                }
            };

            //Act
            var response =
                await client.PostAsync("admin/sensors/createStaticSensor", new FormUrlEncodedContent(payloadDict));

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Post_create_portable_sensor_and_response_ok_status_code_with_correct_content_type(
            int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<Dictionary<string, string>>
            {
                //empty model
                new Dictionary<string, string>(),
                //empty api key
                new Dictionary<string, string>
                {
                    {$"{nameof(CreatePortableSensorViewModel.Model)}.{nameof(CreatePortableSensorModel.ApiKey)}", null}
                }
            };

            var currentDataSet = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            //Act
            var response =
                await client.PostAsync("admin/sensors/createPortableSensor", new FormUrlEncodedContent(currentDataSet));

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_create_portable_sensor_and_response_redirect_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .Build();
            var client = server.CreateClient();

            var payloadDict = new Dictionary<string, string>
            {
                {
                    $"{nameof(CreatePortableSensorViewModel.Model)}.{nameof(CreatePortableSensorModel.ApiKey)}",
                    (CryptoHelper.GenerateApiKey())
                }
            };

            //Act
            var response =
                await client.PostAsync("admin/sensors/createPortableSensor", new FormUrlEncodedContent(payloadDict));

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Get_delete_sensor_and_response_ok_status_code_with_correct_content_type(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.StaticSensor,
                Defaults.PortableSensor
            };

            var currentSensor = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseSensors(currentSensor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["sensorId"] = currentSensor.Id.ToString();

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response = await client.GetAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Get_delete_sensor_and_response_bad_request_status_code_with_correct_content_type()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();


            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
            };

            //Act
            var response = await client.GetAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Get_delete_sensor_and_response_not_found_status_code_with_correct_content_type()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["sensorId"] = "123";

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response = await client.GetAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Get_delete_sensor_and_response_conflict_status_code_with_correct_content_type(
            int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.ActiveStaticSensor,
                Defaults.ActivePortableSensor
            };

            var currentSensor = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseSensors(currentSensor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["sensorId"] = currentSensor.Id.ToString();

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response = await client.GetAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }


        [Theory]
        [InlineData(0, true)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(1, false)]
        public async Task Delete_sensor_and_response_redirect_status_code_with_correct_content_type(int dataSetIndex,
            bool isCompletely)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.StaticSensor,
                Defaults.PortableSensor
            };

            var currentSensor = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseSensors(currentSensor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query[$"{nameof(DeleteSensorViewModel.Model)}.{nameof(DeleteSensorModel.Id)}"] =
                currentSensor.Id.ToString();
            query[$"{nameof(DeleteSensorViewModel.Model)}.{nameof(DeleteSensorModel.IsCompletely)}"] =
                isCompletely.ToString();

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response = await client.DeleteAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Delete_sensor_and_response_bad_request_status_code_with_correct_content_type(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<Dictionary<string, string>>
            {
                //empty model
                new Dictionary<string, string>(),
                //empty id
                new Dictionary<string, string>
                {
                    {$"{nameof(DeleteSensorViewModel.Model)}.{nameof(DeleteSensorModel.Id)}", string.Empty}
                }
            };

            var currentDataSet = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            currentDataSet.ForEach(z => query.Add(z.Key, z.Value));

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response = await client.DeleteAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Delete_sensor_and_response_not_found_status_code_with_correct_content_type()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query[$"{nameof(DeleteSensorViewModel.Model)}.{nameof(DeleteSensorModel.Id)}"] = "123";

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
                Query = query.ToString() ?? string.Empty
            };


            //Act
            var response = await client.DeleteAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Delete_sensor_and_response_conflict_status_code_with_correct_content_type(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.ActiveStaticSensor,
                Defaults.ActivePortableSensor
            };

            var currentSensor = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseSensors(currentSensor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query[$"{nameof(DeleteSensorViewModel.Model)}.{nameof(DeleteSensorModel.Id)}"] =
                currentSensor.Id.ToString();

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response = await client.DeleteAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Get_change_sensor_activation_state_and_response_ok_status_code_with_correct_content_type(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.StaticSensor,
                Defaults.PortableSensor
            };

            var currentSensor = dataSet[dataSetIndex];
            
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .UseSensors(currentSensor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["sensorId"] = currentSensor.Id.ToString();

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/changeActivation",
                Query = query.ToString() ?? string.Empty
            };
            
            //Act
            var response = await client.GetAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Post_change_sensor_activation_state_and_response_redirect_status_code_with_correct_content_type(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<Sensor>
            {
                Defaults.StaticSensor,
                Defaults.PortableSensor
            };

            var currentSensor = dataSet[dataSetIndex];
            
            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .UseSensors(currentSensor)
                .Build();
            var client = server.CreateClient();

            var payload = new Dictionary<string, string>
            {
                {$"{nameof(ChangeActivationSensorViewModel.Model)}.{nameof(ChangeActivationSensorModel.Id)}", currentSensor.Id.ToString()},
            };
            
            //Act
            var response = await client.PostAsync("admin/sensors/changeActivation",  new FormUrlEncodedContent(payload));

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
        
        
        [Fact]
        public async Task Get_change_static_sensor_visibility_state_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            var sensor = Defaults.StaticSensor;

            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .UseSensors(sensor)
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["sensorId"] = sensor.Id.ToString();

            var uriBuilder = new UriBuilder
            {
                Path = "admin/sensors/changeVisibilityStaticSensor",
                Query = query.ToString() ?? string.Empty
            };
            
            //Act
            var response = await client.GetAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_change_static_sensor_visibility_state_and_response_redirect_status_code_with_correct_content_type()
        {
            //Arrange
            var sensor = Defaults.StaticSensor;

            using var server = new TestServerBuilder()
                .UseDefaultAuth()
                .UseSensors(sensor)
                .Build();
            var client = server.CreateClient();

            var payload = new Dictionary<string, string>
            {
                {$"{nameof(ChangeVisibilityStaticSensorViewModel.Model)}.{nameof(ChangeVisibilityStaticSensorModel.Id)}", sensor.Id.ToString()},
            };
            
            //Act
            var response = await client.PostAsync("admin/sensors/changeVisibilityStaticSensor",  new FormUrlEncodedContent(payload));

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
        
    }
}
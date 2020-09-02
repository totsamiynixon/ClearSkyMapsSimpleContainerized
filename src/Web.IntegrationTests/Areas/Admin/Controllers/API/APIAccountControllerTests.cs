using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Web.Areas.Admin.Models.API.Account;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Xunit;

namespace Web.IntegrationTests.Areas.Admin.Controllers.API
{
    [Collection("Integration: Sequential")]
    public class APIAccountControllerTests : BaseScenario
    {
        [Fact]
        public async Task Login_and_response_ok_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsers(user)
                .Build();
            var client = server.CreateClient();

            var model = new LoginModel
            {
                Email = user.Email,
                Password = AdminAreaDefaults.DefaultUserPassword
            };
            var modelJsonStr = JsonConvert.SerializeObject(model);

            //Act
            var response = await client.PostAsync("api/admin/account/login",
                new StringContent(modelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task Login_and_response_bad_request_status_code(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<string>
            {
                //empty model
                string.Empty,
                //model with empty fields
                JsonConvert.SerializeObject(new LoginModel()),
                //model with email without password
                JsonConvert.SerializeObject(new LoginModel {Email = "test@test.com"}),
                //model with password without email
                JsonConvert.SerializeObject(new LoginModel {Password = AdminAreaDefaults.DefaultUserPassword}),
                //model with invalid email
                JsonConvert.SerializeObject(new LoginModel
                    {Email = "email", Password = AdminAreaDefaults.DefaultUserPassword}),
            };

            var currentData = dataSet[dataSetIndex];
            
            using var server = new TestServerBuilder()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/account/login",
                new StringContent(currentData, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task Login_and_response_not_found_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .Build();
            var client = server.CreateClient();

            var model = new LoginModel
            {
                Email = "test@test.com",
                Password = AdminAreaDefaults.DefaultUserPassword
            };
            var modelJsonStr = JsonConvert.SerializeObject(model);

            //Act
            var response = await client.PostAsync("api/admin/account/login",
                new StringContent(modelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task Login_and_response_bad_request_status_code_wrong_password()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsers(user)
                .Build();
            var client = server.CreateClient();

            var model = new LoginModel
            {
                Email = user.Email,
                Password = AdminAreaDefaults.DefaultUserPassword.Reverse().ToString()
            };
            var modelJsonStr = JsonConvert.SerializeObject(model);

            //Act
            var response = await client.PostAsync("api/admin/account/login",
                new StringContent(modelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
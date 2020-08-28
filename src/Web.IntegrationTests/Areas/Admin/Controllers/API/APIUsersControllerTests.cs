using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.API.Users;
using Web.Domain.Entities.Identity;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Xunit;

namespace Web.IntegrationTests.Areas.Admin.Controllers.API
{
    [Collection("Integration: Sequential")]
    public class APIUsersControllerTests : BaseScenario
    {
        [Fact]
        public async Task Get_all_users_and_response_ok_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync("api/admin/users");

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_user_and_response_ok_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var userModel = new CreateUserModel
            {
                Email = "test@test.com",
                Password = "Pass@word911",
                ConfirmPassword = "Pass@word911"
            };
            var userModelJsonStr = JsonConvert.SerializeObject(userModel);

            //Act
            var response = await client.PostAsync("api/admin/users",
                new StringContent(userModelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Create_user_and_response_bad_request_status_code(int dataSetIndex)
        {
            //Arrange

            var dataSet = new List<string>
            {
                string.Empty,
                //no password
                JsonConvert.SerializeObject(new CreateUserModel
                {
                    Email = "test@gmail.com",
                }),
                //wrong email
                JsonConvert.SerializeObject(new CreateUserModel
                {
                    Email = "test",
                    Password = "Pass@word",
                    ConfirmPassword = "Pass@word"
                }),
                //password not match
                JsonConvert.SerializeObject(new CreateUserModel
                {
                    Email = "test@test.com",
                    Password = "Pass@word",
                    ConfirmPassword = "WrongPass@word"
                })
            };

            var currentData = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/users",
                new StringContent(currentData, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_user_and_response_conflict_status_code_when_email_is_already_taken()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;
            var email = user.Email;

            using var server = new TestServerBuilder()
                .UseUsers(user)
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var userModel = new CreateUserModel
            {
                Email = email,
                Password = "Pass@word",
                ConfirmPassword = "Pass@word"
            };
            var userModelJsonStr = JsonConvert.SerializeObject(userModel);

            //Act
            var response = await client.PostAsync("api/admin/users",
                new StringContent(userModelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        
        [Fact]
        public async Task Create_user_and_response_conflict_status_code_when_unable_to_create_user()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var userModel = new CreateUserModel
            {
                Email = "test@test.com",
                Password = "Pass@word",
                ConfirmPassword = "Pass@word"
            };
            var userModelJsonStr = JsonConvert.SerializeObject(userModel);

            //Act
            var response = await client.PostAsync("api/admin/users",
                new StringContent(userModelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Change_user_password_and_response_ok_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsers(user)
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var changePasswordModel = new UserChangePasswordModel
            {
                Id = user.Id,
                Password = "Pass@word911",
                ConfirmPassword = "Pass@word911"
            };
            var changePasswordModelJsonStr = JsonConvert.SerializeObject(changePasswordModel);

            //Act
            var response = await client.PostAsync("api/admin/users/changePassword",
                new StringContent(changePasswordModelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task Change_user_password_and_response_bad_request_status_code(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<string>
            {
                //empty model
                string.Empty,
                //model with empty fields
                JsonConvert.SerializeObject(new UserChangePasswordModel()),
                //model without new password
                JsonConvert.SerializeObject(new UserChangePasswordModel {Id = "123"}),
                //model without id
                JsonConvert.SerializeObject(new UserChangePasswordModel
                    {Password = "Pass@word911", ConfirmPassword = "Pass@word911"}),
                //model with password not match
                JsonConvert.SerializeObject(new UserChangePasswordModel
                    {Id = "123", Password = "Pass@word911", ConfirmPassword = "Pass@word922"}),
            };

            var currentData = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/users/changePassword",
                new StringContent(currentData, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Change_user_password_and_response_not_found_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var changePasswordModel = new UserChangePasswordModel
            {
                Id = "123",
                Password = "Pass@word911",
                ConfirmPassword = "Pass@word911"
            };
            var changePasswordModelJsonStr = JsonConvert.SerializeObject(changePasswordModel);

            //Act
            var response = await client.PostAsync("api/admin/users/changePassword",
                new StringContent(changePasswordModelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Change_user_password_and_response_conflict_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Supervisor}))
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var changePasswordModel = new UserChangePasswordModel
            {
                Id = user.Id,
                Password = "Pass@word911",
                ConfirmPassword = "Pass@word911"
            };
            var changePasswordModelJsonStr = JsonConvert.SerializeObject(changePasswordModel);

            //Act
            var response = await client.PostAsync("api/admin/users/changePassword",
                new StringContent(changePasswordModelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }


        [Fact]
        public async Task Delete_user_and_response_ok_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsers(user)
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var deleteUserModel = new DeleteUserModel {Id = user.Id};
            var deleteUserModelJsonStr = JsonConvert.SerializeObject(deleteUserModel);

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/users")
            {
                Content = new StringContent(deleteUserModelJsonStr, Encoding.UTF8, "application/json")
            });

            //Assert
            response.EnsureSuccessStatusCode();
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Delete_user_and_response_bad_request_status_code(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<string>
            {
                //empty model
                string.Empty,
                //model without id
                JsonConvert.SerializeObject(new DeleteUserModel())
            };

            var currentData = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/users")
            {
                Content = new StringContent(currentData, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_user_and_response_not_found_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var deleteUserModel = new DeleteUserModel {Id = "123"};
            var deleteUserModelJsonStr = JsonConvert.SerializeObject(deleteUserModel);

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/users")
            {
                Content = new StringContent(deleteUserModelJsonStr, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task Delete_user_and_response_conflict_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Supervisor}))
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var deleteUserModel = new DeleteUserModel {Id = user.Id};
            var deleteUserModelJsonStr = JsonConvert.SerializeObject(deleteUserModel);

            //Act
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/admin/users")
            {
                Content = new StringContent(deleteUserModelJsonStr, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Change_user_activation_state_and_response_ok_status_code(bool isActive)
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsers(user)
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var model = new ActivateUserModel {Id = user.Id, IsActive = isActive};
            var modelJsonStr = JsonConvert.SerializeObject(model);

            //Act
            var response = await client.PostAsync("api/admin/users/changeActivation",
                new StringContent(modelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Change_user_activation_state_and_response_bad_request_status_code(int dataSetIndex)
        {
            //Arrange
            var dataSet = new List<string>
            {
                //empty model
                string.Empty,
                //model with empty id
                JsonConvert.SerializeObject(new ActivateUserModel())
            };

            var currentData = dataSet[dataSetIndex];

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.PostAsync("api/admin/users/changeActivation",
                new StringContent(currentData, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Change_user_activation_state_and_response_not_found_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var model = new ActivateUserModel {Id = "123"};
            var modelJsonStr = JsonConvert.SerializeObject(model);

            //Act
            var response = await client.PostAsync("api/admin/users/changeActivation",
                new StringContent(modelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Change_user_activation_state_and_response_conflict_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Supervisor}))
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var model = new ActivateUserModel {Id = user.Id};
            var modelJsonStr = JsonConvert.SerializeObject(model);

            //Act
            var response = await client.PostAsync("api/admin/users/changeActivation",
                new StringContent(modelJsonStr, Encoding.UTF8, "application/json"));

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
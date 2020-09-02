using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.Default.Users;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Xunit;

namespace Web.IntegrationTests.Areas.Admin.Controllers.Default
{
    [Collection("Integration: Sequential")]
    public class UsersControllerTests : BaseScenario
    {
        [Theory]
        [InlineData("admin/users")]
        [InlineData("admin/users/index")]
        public async Task Get_get_all_users_and_response_ok_status_code_with_correct_content_type(string url)
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
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
        public async Task Get_create_user_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync("admin/users/create");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_create_user_and_response_redirect_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .Build();
            var client = server.CreateClient();

            var payloadDict = new Dictionary<string, string>
            {
                {
                    $"{nameof(CreateUserModel.Email)}", "test@test.com"
                },
                {
                    $"{nameof(CreateUserModel.Password)}", "FakePassword@123"
                },
                {
                    $"{nameof(CreateUserModel.ConfirmPassword)}", "FakePassword@123"
                },
            };

            //Act
            var response =
                await client.PostAsync("admin/users/create", new FormUrlEncodedContent(payloadDict));

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task Get_change_password_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Admin}))
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["userId"] = user.Id;

            var uriBuilder = new UriBuilder
            {
                Path = "admin/users/changePassword",
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
        public async Task Post_change_user_password_and_response_redirect_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Admin}))
                .Build();
            var client = server.CreateClient();

            var payloadDict = new Dictionary<string, string>
            {
                {
                    $"{nameof(UserChangePasswordViewModel.Model)}.{nameof(UserChangePasswordModel.Id)}", user.Id
                },
                {
                    $"{nameof(UserChangePasswordViewModel.Model)}.{nameof(UserChangePasswordModel.Password)}",
                    "FakePassword@123"
                },
                {
                    $"{nameof(UserChangePasswordViewModel.Model)}.{nameof(UserChangePasswordModel.ConfirmPassword)}",
                    "FakePassword@123"
                },
            };

            //Act
            var response =
                await client.PostAsync("admin/users/changePassword", new FormUrlEncodedContent(payloadDict));

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }


        [Fact]
        public async Task Get_delete_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Admin}))
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["userId"] = user.Id;

            var uriBuilder = new UriBuilder
            {
                Path = "admin/users/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response = await client.DeleteAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Delete_user_and_response_redirect_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Admin}))
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query[$"{nameof(DeleteUserViewModel.Model)}.{nameof(DeleteUserModel.Id)}"] = user.Id;

            var uriBuilder = new UriBuilder
            {
                Path = "admin/users/delete",
                Query = query.ToString() ?? string.Empty
            };

            //Act
            var response =
                await client.DeleteAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
        
        
        [Fact]
        public async Task Get_change_activation_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Admin}))
                .Build();
            var client = server.CreateClient();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["userId"] = user.Id;

            var uriBuilder = new UriBuilder
            {
                Path = "admin/users/changeActivation",
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
        public async Task Post_change_activation_user_and_response_redirect_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseCustomAuth(AdminAreaDefaults.DefaultUser, AuthSettings.Roles.Supervisor)
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Admin}))
                .Build();
            var client = server.CreateClient();

            var payload = new Dictionary<string, string>
            {
                {$"{nameof(ActivateUserViewModel.Model)}.{nameof(ActivateUserModel.Id)}", user.Id}
            };

            //Act
            var response =
                await client.PostAsync("admin/users/changeActivation", new FormUrlEncodedContent(payload));

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}
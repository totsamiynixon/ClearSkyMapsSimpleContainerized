using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.API.Account;
using Web.Infrastructure.AntiForgery;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Web.IntegrationTests.Infrastructure.AntiForgery;
using Xunit;

namespace Web.IntegrationTests.Areas.Admin.Controllers.Default
{
    [Collection("Integration: Sequential")]
    public class AccountControllerTests : BaseScenario
    {
        [Fact]
        public async Task Get_login_and_response_ok_status_code_with_correct_content_type()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .Build();
            var client = server.CreateClient();
            var uriBuilder = new UriBuilder
            {
                Path = "admin/account/login",
            };

            //Act
            var response = await client.GetAsync(uriBuilder.Uri.PathAndQuery);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }


        [Fact]
        public async Task Post_login_and_response_redirect_status_code()
        {
            //Arrange
            var user = AdminAreaDefaults.DefaultUser;

            using var server = new TestServerBuilder()
                .UseUsersWithRoles((user, new List<string> {AuthSettings.Roles.Admin}))
                .Build();
            var client = server.CreateClient();

            var initResponse = await client.GetAsync("admin/account/login");
            var antiForgeryValues = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(initResponse);

            client.DefaultRequestHeaders.Add(HeaderNames.Cookie,
                new CookieHeaderValue(AntiForgerySettings.AntiForgeryCookieName, antiForgeryValues.cookieValue)
                    .ToString());

            var payload = new Dictionary<string, string>
            {
                {nameof(LoginModel.Email), user.Email},
                {nameof(LoginModel.Password), AdminAreaDefaults.DefaultUserPassword},
                {AntiForgerySettings.AntiForgeryFieldName, antiForgeryValues.fieldValue}
            };

            //Act
            var response =
                await client.PostAsync("admin/account/login", new FormUrlEncodedContent(payload));
            response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains(cookies, z => z.Contains(AuthSettings.CookieName));
        }

        [Fact]
        public async Task Get_logout_and_response_redirect_status_code()
        {
            //Arrange
            using var server = new TestServerBuilder()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync("admin/account/logoff");

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}
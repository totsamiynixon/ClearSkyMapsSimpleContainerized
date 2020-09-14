using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Web.Domain.Entities.Identity;

namespace Web.IntegrationTests.Areas.Admin.Infrastructure.Auth
{
    public class TestAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public string AuthenticationScheme { get; set; }
        
        public User User { get; set; }
        
        public List<string> Roles { get; set; }
    }
}
using Microsoft.AspNetCore.Identity;
using Web.Domain.Entities.Identity;

namespace Web.IntegrationTests.Areas.Admin.Infrastructure
{
    public static class AdminAreaDefaults
    {
        public const string DefaultUserPassword = "qwerty";
        public static User DefaultUser
        {
            get
            {
                var user = new User
                {
                    UserName = "test@test.com",
                    NormalizedUserName = "TEST@TEST.COM",
                    Email = "test@test.com",
                    NormalizedEmail = "TEST@TEST.COM",
                    IsActive = true
                };
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, DefaultUserPassword);
                return user;
            }
        }
    }
}
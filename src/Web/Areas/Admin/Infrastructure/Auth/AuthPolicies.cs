using Microsoft.AspNetCore.Authorization;

namespace Web.Areas.Admin.Infrastructure.Auth
{
    public class AuthPolicies
    {
        public const string Admin =  AuthSettings.Roles.Admin;

        public const string Supervisor = AuthSettings.Roles.Supervisor;

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(AuthSettings.Roles.Admin, AuthSettings.Roles.Supervisor)
                .Build();
        }

        public static AuthorizationPolicy SupervisorPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(AuthSettings.Roles.Supervisor)
                .Build();
        }
    }
}
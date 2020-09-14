namespace Web.Areas.Admin.Infrastructure.Auth
{
    public class AuthSettings
    {
        public const string CookieName = "CSM.Admin.Auth";
        public class Roles
        {
            public const string Supervisor = nameof(Supervisor);
            
            public const string Admin = nameof(Admin);
        }
    }
}
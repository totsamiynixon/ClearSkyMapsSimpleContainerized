using System.Collections.Generic;
using Web.Domain.Entities.Identity;

namespace Web.IntegrationTests.Areas.Admin.Infrastructure
{
    public class TestAdminAreaOptions
    {
        public TestAdminAreaAuthOptions Auth { get; set; }
    }

    public class TestAdminAreaAuthOptions
    {
        public bool UseCustomAuth { get; set; }
        
        public User User { get; set; }
        
        public List<string> Roles { get; set; }
    }
}
using Microsoft.AspNetCore.Identity;

namespace Web.Domain.Entities.Identity
{
    public class User: IdentityUser
    {
        public bool IsActive { get; set; }
    }
}

using System.Collections.Generic;

namespace Web.Areas.Admin.Application.Users.Queries.DTO
{
    public class UserDetailsDTO
    {
        public string Id { get; set; }
        
        public string Email { get; set; }
        
        public IEnumerable<UserRoleDTO> Roles { get; set; }
    }
}
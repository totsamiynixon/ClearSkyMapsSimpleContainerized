using System.Collections.Generic;

namespace Web.Areas.Admin.Models.API.Account
{
    public class UserDetailsModel
    {
        public string Id { get; set; }
        
        public string UserName { get; set; }
        
        public string Email { get; set; }
        
        public IEnumerable<string> Roles { get; set; }
    }
}
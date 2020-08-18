namespace Web.Areas.Admin.Models.API.Account
{
    public class TokenModel
    {
        public string Token { get; set; }
        
        public UserDetailsModel UserDetails { get; set; }
    }
}
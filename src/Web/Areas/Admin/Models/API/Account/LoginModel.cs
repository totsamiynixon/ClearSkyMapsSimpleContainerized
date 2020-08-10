using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.API.Account
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Адрес электронной почты")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить вас?")]
        public bool RememberMe { get; set; }
    }
}
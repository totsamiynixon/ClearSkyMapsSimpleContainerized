using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Default.Users
{
    public class CreateUserModel
    {
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите Пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают, проверьте свой ввод!")]
        public string ConfirmPassword { get; set; }
    }
}
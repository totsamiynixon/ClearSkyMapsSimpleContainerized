using MediatR;

namespace Web.Areas.Admin.Application.Users.Commands
{
    public class ChangeUserPasswordCommand : IRequest<bool>
    {
        public string UserId { get; }

        public string Password { get; }

        public string ConfirmPassword { get; }

        public ChangeUserPasswordCommand(string userId, string password, string confirmPassword)
        {
            UserId = userId;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
}
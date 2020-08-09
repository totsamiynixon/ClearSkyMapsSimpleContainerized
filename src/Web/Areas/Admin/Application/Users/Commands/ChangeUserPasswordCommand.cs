using MediatR;

namespace Web.Areas.Admin.Application.Users.Commands
{
    public class ChangeUserPasswordCommand : IRequest<bool>
    {
        public string UserId { get;}
        
        public string Email { get; }
        
        public string Password { get; }
        
        public string ConfirmPassword { get; }

        public ChangeUserPasswordCommand(string userId, string email, string password, string confirmPassword)
        {
            UserId = userId;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
    ChangeUserActivationStateCommand
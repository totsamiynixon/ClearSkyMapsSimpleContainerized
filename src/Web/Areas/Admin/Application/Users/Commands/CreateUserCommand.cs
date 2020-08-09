using MediatR;

namespace Web.Areas.Admin.Application.Users.Commands
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string Email { get; }
        
        public string Password { get; }
        
        public CreateUserCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
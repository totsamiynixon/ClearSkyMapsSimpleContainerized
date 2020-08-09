using MediatR;

namespace Web.Areas.Admin.Application.Users.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public DeleteUserCommand(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
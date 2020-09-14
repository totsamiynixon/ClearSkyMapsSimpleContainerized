using MediatR;

namespace Web.Areas.Admin.Application.Users.Commands
{
    public class ChangeUserActivationStateCommand : IRequest<bool>
    {
        public string UserId { get; }
        
        public bool IsActive { get; }

        public ChangeUserActivationStateCommand(string userId, bool isActive)
        {
            UserId = userId;
            IsActive = isActive;
        }
    }
}
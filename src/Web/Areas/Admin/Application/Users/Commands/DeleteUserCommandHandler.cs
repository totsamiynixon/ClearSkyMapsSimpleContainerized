using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Domain.Entities.Identity;

namespace Web.Areas.Admin.Application.Users.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public DeleteUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == request.UserId, cancellationToken: cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(request.UserId);
            }
            if (await _userManager.IsInRoleAsync(user, AuthSettings.Roles.Supervisor))
            {
                throw new UserUnableToChangeStateException(
                    $"Unable to change password for user with role {AuthSettings.Roles.Supervisor}");
            }
            await _userManager.DeleteAsync(user);

            return true;
        }
    }
}
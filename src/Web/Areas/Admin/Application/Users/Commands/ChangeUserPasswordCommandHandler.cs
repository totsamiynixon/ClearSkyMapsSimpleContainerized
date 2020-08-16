using System;
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
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public ChangeUserPasswordCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<bool> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            if (await _userManager.IsInRoleAsync(user, AuthSettings.Roles.Supervisor))
            {
                throw new UserUnableToChangeStateException(
                    $"Unable to change password for user with role {AuthSettings.Roles.Supervisor}");
            }

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
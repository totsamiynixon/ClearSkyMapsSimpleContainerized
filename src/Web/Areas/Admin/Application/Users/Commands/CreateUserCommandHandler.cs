using System;
using System.Linq;
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
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public CreateUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                throw new UserEmailAddressIsAlreadyTakenException(request.Email);
            }

            var newUser = new User
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                throw new UserUnableToCreateException(result.Errors.First().Description);
            }

            await _userManager.AddToRoleAsync(newUser, AuthSettings.Roles.Admin);

            return true;
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Domain.Entities.Identity;

namespace Web.Areas.Admin.Application.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public CreateUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Email == request.Email, cancellationToken);
            if (user != null)
            {
                throw new UserEmailAddressIsAlreadyTakenException();
            }
            var newUser = new User
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Admin");
            }
        }
    }
}
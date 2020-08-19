using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Web.Areas.Admin.Application.Users.Commands;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Domain.Entities.Identity;
using Web.Infrastructure.Data;
using Xunit;

namespace Web.Tests.Functional.Unit.Areas.Admin.Application.Users.Commands
{
    public class CreateUserCommandHandlerTest
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IdentityDataContext> _identityDataContextMock;

        public CreateUserCommandHandlerTest()
        {
            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null,
                null, null, null, null);
            _identityDataContextMock = new Mock<IdentityDataContext>(new DbContextOptions<IdentityDataContext>());
        }

        [Fact]
        public async Task Handler_should_throw_exception_if_dependency_is_null()
        {
            //Arrage

            //Act
            void Act()
            {
                var command = new CreateUserCommandHandler(null);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_return_true()
        {
            //Arrange
            var email = "123@gmail.com";
            var password = "123";
            var fakeUserDbSet = new List<User> { };

            _userManagerMock.Setup(x => x.FindByEmailAsync(Guid.NewGuid().ToString()))
                .Returns(Task.FromResult<User>(null));
            _userManagerMock.Setup(x => x.CreateAsync(It.Is<User>(it => it.Email == email), password))
                .Returns(Task.FromResult(IdentityResult.Success));

            var cancellationToken = new CancellationToken();
            var command = new CreateUserCommand(email, password);
            var handler =
                new CreateUserCommandHandler(_userManagerMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handler_should_throw_email_taken_exception_if_user_already_exists()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = "123@gmail.com"
            };
            var fakeUserDbSet = new List<User> {fakeUser};

            _userManagerMock.Setup(x => x.FindByEmailAsync(fakeUser.Email)).Returns(Task.FromResult(fakeUser));

            var cancellationToken = new CancellationToken();
            var command = new CreateUserCommand(fakeUser.Email, "123");
            var handler =
                new CreateUserCommandHandler(_userManagerMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<UserEmailAddressIsAlreadyTakenException>(Act);
        }
        
        [Fact]
        public async Task Handler_should_should_throw_unable_to_add_user_exception()
        {
            //Arrange
            var email = "123@gmail.com";
            var password = "123";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).Returns(Task.FromResult<User>(null));
            _userManagerMock.Setup(x => x.CreateAsync(It.Is<User>(it => it.Email == email), password))
                .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError())));

            var cancellationToken = new CancellationToken();
            var command = new CreateUserCommand(email, password);
            var handler =
                new CreateUserCommandHandler(_userManagerMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<UserUnableToCreateException>(Act);
        }

        [Fact]
        public async Task Handler_user_should_be_created()
        {
            //Arrange
            var email = "123@gmail.com";
            var password = "123";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).Returns(Task.FromResult<User>(null));
            _userManagerMock.Setup(x => x.CreateAsync(It.Is<User>(it => it.Email == email), password))
                .Returns(Task.FromResult(IdentityResult.Success));

            var cancellationToken = new CancellationToken();
            var command = new CreateUserCommand(email, password);
            var handler =
                new CreateUserCommandHandler(_userManagerMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            _userManagerMock.Verify(
                x => x.CreateAsync(It.Is<User>(it => it.Email == email), It.Is<string>(it => it == password)),
                Times.Once);
        }
        
        [Fact]
        public async Task Handler_should_add_role_admin_to_user()
        {
            //Arrange
            var email = "123@gmail.com";
            var password = "123";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).Returns(Task.FromResult<User>(null));
            _userManagerMock.Setup(x => x.CreateAsync(It.Is<User>(it => it.Email == email), password))
                .Returns(Task.FromResult(IdentityResult.Success));

            var cancellationToken = new CancellationToken();
            var command = new CreateUserCommand(email, password);
            var handler =
                new CreateUserCommandHandler(_userManagerMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            _userManagerMock.Verify(
                x => x.AddToRoleAsync(It.Is<User>(it => it.Email == email), It.Is<string>(it => it == AuthSettings.Roles.Admin)),
                Times.Once);
        }
    }
}
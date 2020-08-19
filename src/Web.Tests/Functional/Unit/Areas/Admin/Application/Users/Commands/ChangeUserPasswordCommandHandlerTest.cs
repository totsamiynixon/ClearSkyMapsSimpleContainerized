using System;
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
    public class ChangeUserPasswordCommandHandlerTest
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IdentityDataContext> _identityDataContextMock;

        public ChangeUserPasswordCommandHandlerTest()
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
                var command = new ChangeUserPasswordCommandHandler(null);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_return_true()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString()
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult(fakeUser));
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserPasswordCommand(fakeUser.Id, "123", "123");
            var handler =
                new ChangeUserPasswordCommandHandler(_userManagerMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_user_doesnt_exist()
        {
            //Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync(Guid.NewGuid().ToString())).Returns(Task.FromResult<User>(null));
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserPasswordCommand("123", "123", "123");
            var handler =
                new ChangeUserPasswordCommandHandler(_userManagerMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(Act);
        }


        [Fact]
        public async Task Handler_should_throw_unable_to_change_state_exception_if_user_is_supervisor()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString()
            };


            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult(fakeUser));
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(true));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserPasswordCommand(fakeUser.Id, "123", "123");
            var handler =
                new ChangeUserPasswordCommandHandler(_userManagerMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<UserUnableToChangeStateException>(Act);
        }

        [Fact]
        public async Task Handler_should_update_password_for_user()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString(),
            };
            var password = "123";
            var passwordHash = new PasswordHasher<User>().HashPassword(fakeUser, password);
            
            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult(fakeUser));
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserPasswordCommand(fakeUser.Id, password, password);
            var handler =
                new ChangeUserPasswordCommandHandler(_userManagerMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            Assert.NotEqual(passwordHash, fakeUser.PasswordHash);
        }

        [Fact]
        public async Task Handler_manager_should_update_user()
        {
            //Arrange
            var password = "123";
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString(),
            };


            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult(fakeUser));
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserPasswordCommand(fakeUser.Id, password, password);
            var handler =
                new ChangeUserPasswordCommandHandler(_userManagerMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<User>(it => it == fakeUser)), Times.Once);
        }
    }
}
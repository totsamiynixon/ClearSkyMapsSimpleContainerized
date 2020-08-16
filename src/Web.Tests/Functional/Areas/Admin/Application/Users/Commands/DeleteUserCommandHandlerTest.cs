using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Web.Areas.Admin.Application.Users.Commands;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Domain.Entities.Identity;
using Xunit;

namespace Web.Tests.Functional.Areas.Admin.Application.Users.Commands
{
    public class DeleteUserCommandHandlerTest
    {
        private readonly Mock<UserManager<User>> _userManagerMock;

        public DeleteUserCommandHandlerTest()
        {
            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null,
                null, null, null, null);
        }

        [Fact]
        public async Task Handler_should_throw_exception_if_dependency_is_null()
        {
            //Arrage

            //Act
            void Act()
            {
                var command = new DeleteUserCommandHandler(null);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_user_doesnt_exist()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).Returns(Task.FromResult<User>(null));

            var cancellationToken = new CancellationToken();
            var command = new DeleteUserCommand(userId);
            var handler =
                new DeleteUserCommandHandler(_userManagerMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(Act);
        }

        [Fact]
        public async Task Handler_should_return_true()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString()
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult<User>(fakeUser));
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new DeleteUserCommand(fakeUser.Id);
            var handler =
                new DeleteUserCommandHandler(_userManagerMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handler_should_throw_unable_to_change_state_exception_if_user_is_supervisor()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString()
            };


            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult<User>(fakeUser));
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(true));

            var cancellationToken = new CancellationToken();
            var command = new DeleteUserCommand(fakeUser.Id);
            var handler =
                new DeleteUserCommandHandler(_userManagerMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<UserUnableToChangeStateException>(Act);
        }

        [Fact]
        public async Task Handler_should_delete_user_if_not_supervisor()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString(),
            };


            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult<User>(fakeUser));
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new DeleteUserCommand(fakeUser.Id);
            var handler =
                new DeleteUserCommandHandler(_userManagerMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            _userManagerMock.Verify(x => x.DeleteAsync(It.Is<User>(it => it == fakeUser)), Times.Once);
        }
    }
}
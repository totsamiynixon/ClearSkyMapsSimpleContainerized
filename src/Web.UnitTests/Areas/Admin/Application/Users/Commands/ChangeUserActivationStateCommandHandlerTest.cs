using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Areas.Admin.Application.Users.Commands;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Infrastructure.Data;
using Web.Domain.Entities.Identity;
using Xunit;

namespace Web.UnitTests.Areas.Admin.Application.Users.Commands
{
    public class ChangeUserActivationStateCommandHandlerTest
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IdentityDataContext> _identityDataContextMock;

        public ChangeUserActivationStateCommandHandlerTest()
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
                var command = new ChangeUserActivationStateCommandHandler(null);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_user_doesnt_exist()
        {
            //Arrange
            _identityDataContextMock.Setup(x => x.Users).ReturnsDbSet(new List<User>());
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserActivationStateCommand("322", true);
            var handler =
                new ChangeUserActivationStateCommandHandler(_userManagerMock.Object);

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

            _identityDataContextMock.Setup(x => x.Users).ReturnsDbSet(new List<User> {fakeUser});
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserActivationStateCommand(fakeUser.Id, true);
            var handler =
                new ChangeUserActivationStateCommandHandler(_userManagerMock.Object);

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


            _identityDataContextMock.Setup(x => x.Users).ReturnsDbSet(new List<User> {fakeUser});
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(true));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserActivationStateCommand(fakeUser.Id, true);
            var handler =
                new ChangeUserActivationStateCommandHandler(_userManagerMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<UserUnableToChangeStateException>(Act);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handler_should_change_user_active_state_if_not_supervisor(bool newActiveState)
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                IsActive = !newActiveState
            };


            _identityDataContextMock.Setup(x => x.Users).ReturnsDbSet(new List<User> {fakeUser});
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserActivationStateCommand(fakeUser.Id, newActiveState);
            var handler =
                new ChangeUserActivationStateCommandHandler(_userManagerMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.Equal(newActiveState, fakeUser.IsActive);
        }

        [Fact]
        public async Task Handler_should_call_user_manager_update()
        {
            //Arrange
            var fakeUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                IsActive = false
            };


            _identityDataContextMock.Setup(x => x.Users).ReturnsDbSet(new List<User> {fakeUser});
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);
            _userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, AuthSettings.Roles.Supervisor))
                .Returns(Task.FromResult(false));

            var cancellationToken = new CancellationToken();
            var command = new ChangeUserActivationStateCommand(fakeUser.Id, !fakeUser.IsActive);
            var handler =
                new ChangeUserActivationStateCommandHandler(_userManagerMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<User>(it => it == fakeUser)), Times.Once);
        }
    }
}
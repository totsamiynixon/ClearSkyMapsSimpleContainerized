using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Areas.Admin.Application.Users.Queries;
using Web.Areas.Admin.Application.Users.Queries.DTO;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Infrastructure.Data;
using Web.Domain.Entities.Identity;
using Xunit;

namespace Web.UnitTests.Areas.Admin.Application.Users.Queries
{
    public class UserQueriesTest
    {
        private readonly Mock<IdentityDataContext> _identityDataContextMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;

        public UserQueriesTest()
        {
            _identityDataContextMock = new Mock<IdentityDataContext>(new DbContextOptions<IdentityDataContext>());
            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null,
                null, null, null, null);
            _mapperMock = new Mock<IMapper>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Handler_should_throw_exception_if_dependency_is_null(int paramsSetIndex)
        {
            //Arrage
            var testArgs = new[]
            {
                new[] {(object) null, _mapperMock.Object},
                new[] {_userManagerMock.Object, (object) null},
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var queries = new UserQueries((UserManager<User>) testArgsCurrentSet[0],
                    (IMapper) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_return_users_dto()
        {
            //Arrange
            var fakeUser = new User {Id = Guid.NewGuid().ToString()};
            var fakeUserDbSet = new List<User> {fakeUser};

            _identityDataContextMock.Setup(x => x.Users).ReturnsDbSet(fakeUserDbSet);
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);
            _mapperMock.Setup(x => x.Map<List<User>, List<UserListItemDTO>>(It.IsAny<List<User>>())).Returns(
                (List<User> value) =>
                {
                    return value.Select(z => new UserListItemDTO
                    {
                        Id = z.Id
                    }).ToList();
                });

            var queries = new UserQueries(_userManagerMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetUsersAsync();

            //Assert
            Assert.Equal(fakeUser.Id, result.First().Id);
        }

        [Fact]
        public async Task Handler_should_return_empty_list_if_database_is_empty()
        {
            //Arrange
            _identityDataContextMock.Setup(x => x.Users).ReturnsDbSet(new List<User>());
            _userManagerMock.Setup(x => x.Users).Returns(_identityDataContextMock.Object.Users);
            _mapperMock.Setup(x => x.Map<List<User>, List<UserListItemDTO>>(It.IsAny<List<User>>())).Returns(
                (List<User> value) =>
                {
                    return value.Select(z => new UserListItemDTO
                    {
                        Id = z.Id
                    }).ToList();
                });

            var queries = new UserQueries(_userManagerMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetUsersAsync();

            //Assert
            Assert.False(result.Any());
        }

        [Fact]
        public async Task Handler_should_return_null_if_database_is_empty()
        {
            //Arrange
            var userIdThatDoesntExist = Guid.NewGuid().ToString();
            
            _userManagerMock.Setup(x => x.FindByIdAsync(userIdThatDoesntExist)).Returns(Task.FromResult<User>(null));


            var queries = new UserQueries(_userManagerMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetUserAsync(userIdThatDoesntExist);

            //Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(AuthSettings.Roles.Admin)]
        [InlineData(AuthSettings.Roles.Supervisor)]
        public async Task Handler_should_return_user_dto_with_roles(string role)
        {
            //Arrange
            var fakeUser = new User {Id = Guid.NewGuid().ToString()};
            var fakeUserDTO = new UserDetailsDTO {Id = fakeUser.Id};
            IList<string> roles = new List<string> {role};

            _userManagerMock.Setup(x => x.GetRolesAsync(fakeUser)).Returns(Task.FromResult(roles));
            _userManagerMock.Setup(x => x.FindByIdAsync(fakeUser.Id)).Returns(Task.FromResult(fakeUser));
            _mapperMock.Setup(x => x.Map<User, UserDetailsDTO>(fakeUser)).Returns(fakeUserDTO);


            var queries = new UserQueries(_userManagerMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetUserAsync(fakeUser.Id);

            //Assert
            Assert.Equal(fakeUserDTO, result);
            Assert.True(result.Roles.Select(z => z.Name).SequenceEqual(roles));
        }
    }
}
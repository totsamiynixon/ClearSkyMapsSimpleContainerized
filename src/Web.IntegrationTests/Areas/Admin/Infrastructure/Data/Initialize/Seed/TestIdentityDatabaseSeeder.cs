using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Infrastructure.Data;
using Web.Domain.Entities.Identity;
using Web.Infrastructure.Data.Initialize.Seed;

namespace Web.IntegrationTests.Areas.Admin.Infrastructure.Data.Initialize.Seed
{
    public class TestIdentityDatabaseSeeder : IDatabaseSeeder<IdentityDataContext>
    {
        private readonly (User user, List<string> roles)[] _usersWithRoles;

        public TestIdentityDatabaseSeeder(params User[] users)
        {
            _usersWithRoles = users.Select(z => (z, new List<string>())).ToArray();
        }

        public TestIdentityDatabaseSeeder(params (User, List<string>)[] usersWithRolesWithRoles)
        {
            _usersWithRoles = usersWithRolesWithRoles;
        }

        public async Task SeedAsync(IdentityDataContext context)
        {
            foreach (var userWithRole in _usersWithRoles)
            {
                context.Users.Add(userWithRole.user);
                if (userWithRole.roles.Any())
                {
                    var rolesIds = await context.Roles
                        .AsQueryable()
                        .Where(role => userWithRole.roles.Any(ur => ur == role.Name))
                        .Select(z => z.Id)
                        .ToArrayAsync();
                    var userRoles = rolesIds.Select(r => new IdentityUserRole<string>
                    {
                        RoleId = r,
                        UserId = userWithRole.user.Id
                    });
                    context.UserRoles.AddRange(userRoles);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
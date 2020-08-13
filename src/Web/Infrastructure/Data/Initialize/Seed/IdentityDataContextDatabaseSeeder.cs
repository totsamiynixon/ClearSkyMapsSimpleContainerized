using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Web.Domain.Entities.Identity;

namespace Web.Infrastructure.Data.Initialize.Seed
{
    public class IdentityDataContextDatabaseSeeder : IDatabaseSeeder<IdentityDataContext>
    {
        public async Task SeedAsync(IdentityDataContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.Add(new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });

                var supervisorRole = new IdentityRole
                {
                    Name = "Supervisor",
                    NormalizedName = "SUPERVISOR"
                };
                context.Roles.Add(supervisorRole);

                var hasher = new PasswordHasher<User>();
                var userAdmin = new User
                {
                    Email = "supervisor@clearskymaps.com",
                    NormalizedEmail = "SUPERVISOR@CLEARSKYMAPS.COM",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "supervisor@clearskymaps.com",
                    NormalizedUserName = "SUPERVISOR@CLEARSKYMAPS.COM",
                    IsActive = true
                };
                context.Users.Add(userAdmin);
                await context.SaveChangesAsync();

                userAdmin.PasswordHash = hasher.HashPassword(userAdmin, "VerySecurePassword");
                context.Entry(userAdmin).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var userRoleSet = context.Set<IdentityUserRole<string>>();
                userRoleSet.Add(new IdentityUserRole<string>
                {
                    RoleId = supervisorRole.Id,
                    UserId = userAdmin.Id
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
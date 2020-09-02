using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities.Identity;

namespace Web.Areas.Admin.Infrastructure.Data
{
    public class IdentityDataContext : IdentityDbContext<User, IdentityRole, string>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(AdminArea.Name);
            
            base.OnModelCreating(builder);
        }
    }
}

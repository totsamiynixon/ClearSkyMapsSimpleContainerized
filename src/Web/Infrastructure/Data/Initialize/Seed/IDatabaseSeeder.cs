using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Initialize.Seed
{
    public interface IDatabaseSeeder<TContext> where TContext : DbContext
    {
        Task SeedAsync(TContext context);
    }
}
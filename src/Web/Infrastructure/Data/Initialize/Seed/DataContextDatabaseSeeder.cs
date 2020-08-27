using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Helpers;
using Web.Infrastructure.Data.Migrations;

namespace Web.Infrastructure.Data.Initialize.Seed
{
    public class DataContextDatabaseSeeder : IDatabaseSeeder<DataContext>
    {
        public async Task SeedAsync(DataContext context)
        {
            var appliedMigrations = (await context.Database.GetAppliedMigrationsAsync()).ToArray();
            if (appliedMigrations.Any() && appliedMigrations.Last().Contains(nameof(AddApiKey)))
            {
                var sensors = context.Sensors.Where(f => f.ApiKey == null).ToList();
                foreach (var sensor in sensors)
                {
                    sensor.ApiKey = CryptoHelper.GenerateApiKey();
                    context.Entry(sensor).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
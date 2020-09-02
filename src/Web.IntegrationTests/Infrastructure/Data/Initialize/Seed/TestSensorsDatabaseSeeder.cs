using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Initialize.Seed;

namespace Web.IntegrationTests.Infrastructure.Data.Initialize.Seed
{
    public class TestSensorsDatabaseSeeder : IDatabaseSeeder<DataContext>
    {
        public readonly List<Sensor> Sensors;
            
        public TestSensorsDatabaseSeeder(params Sensor[] sensors)
        {
            Sensors = sensors.ToList();
        }
        public async Task SeedAsync(DataContext context)
        {
            try
            {
                await context.Sensors.AddRangeAsync(Sensors);
                await context.SaveChangesAsync();
            }
            catch
            {
                    
            }
        }
    }
}
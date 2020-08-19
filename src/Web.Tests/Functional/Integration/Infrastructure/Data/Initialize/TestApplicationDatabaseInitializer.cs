using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.Data.Initialize.Seed;

namespace Web.Tests.Functional.Integration.Infrastructure.Data.Initialize
{
    public class TestApplicationDatabaseInitializer : DefaultApplicationDatabaseInitializer
    {
        public TestApplicationDatabaseInitializer(IDataContextFactory<DataContext> dataContextFactory,
            IDataContextFactory<IdentityDataContext> identityDataContextFactory,
            IEnumerable<IDatabaseSeeder<DataContext>> databaseSeeders,
            IEnumerable<IDatabaseSeeder<IdentityDataContext>> identityDatabaseSeeders) :
            base(dataContextFactory, identityDataContextFactory, databaseSeeders, identityDatabaseSeeders)
        {
        }

        public override async Task InitializeDbAsync()
        {
            await using var context = _dataContextFactory.Create();
            await context.Database.CloseConnectionAsync();
            await context.Database.OpenConnectionAsync();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            foreach (var seeder in _databaseSeeders)
            {
                await seeder.SeedAsync(context);   
            }

            await using var identityContext = _identityDataContextFactory.Create();
            await identityContext.Database.CloseConnectionAsync();
            await identityContext.Database.OpenConnectionAsync();
            await identityContext.Database.EnsureDeletedAsync();
            await identityContext.Database.EnsureCreatedAsync();

            foreach (var seeder in _identityDatabaseSeeders)
            {
                await seeder.SeedAsync(identityContext);   
            }
        }
    }
}
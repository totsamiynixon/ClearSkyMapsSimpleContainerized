using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.Data.Initialize.Seed;

namespace Web.IntegrationTests.Infrastructure.Data.Initialize
{
    public class TestApplicationDatabaseInitializer : DefaultApplicationDatabaseInitializer
    {
        private static readonly object Lock = new object();

        public TestApplicationDatabaseInitializer(IDataContextFactory<DataContext> dataContextFactory,
            IEnumerable<IDatabaseSeeder<DataContext>> databaseSeeders) :
            base(dataContextFactory, databaseSeeders)
        {
        }

        public override async Task InitializeDbAsync()
        {
            //TODO: изучить, как запускаются тесты и как правильно инициализировать базу
            lock (Lock)
            {
                using (var context = _dataContextFactory.Create())
                {
                    context.Database.EnsureDeleted();
                    /*context.Database.EnsureCreated();*/
                }
            }


            base.InitializeDbAsync().Wait();
        }
    }
}
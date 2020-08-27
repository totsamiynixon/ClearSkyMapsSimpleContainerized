using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize.Seed;

namespace Web.Infrastructure.Data.Initialize
{
    public class DefaultApplicationDatabaseInitializer : IApplicationDatabaseInitializer
    {
        private static readonly object Lock = new object();

        protected readonly IDataContextFactory<DataContext> _dataContextFactory;
        protected readonly IEnumerable<IDatabaseSeeder<DataContext>> _databaseSeeders;

        public DefaultApplicationDatabaseInitializer(IDataContextFactory<DataContext> dataContextFactory,
            IEnumerable<IDatabaseSeeder<DataContext>> databaseSeeders)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _databaseSeeders = databaseSeeders ?? throw new ArgumentNullException(nameof(databaseSeeders));
        }


        public virtual async Task InitializeDbAsync()
        {
            lock (Lock)
            {
                using (var context = _dataContextFactory.Create())
                {
                    try
                    {
                        context.Database.Migrate();
                        foreach (var seeder in _databaseSeeders)
                        {
                            seeder.SeedAsync(context).Wait();
                        }
                    }
                    catch (SqlException exception) when (exception.Number == 1801)
                    {
                        // retry
                    }
                }
            }
        }
    }
}
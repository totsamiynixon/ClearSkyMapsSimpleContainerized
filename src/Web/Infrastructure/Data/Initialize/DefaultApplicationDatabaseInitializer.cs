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
        protected readonly IDataContextFactory<IdentityDataContext> _identityDataContextFactory;
        protected readonly IEnumerable<IDatabaseSeeder<DataContext>> _databaseSeeders;
        protected readonly IEnumerable<IDatabaseSeeder<IdentityDataContext>> _identityDatabaseSeeders;

        public DefaultApplicationDatabaseInitializer(IDataContextFactory<DataContext> dataContextFactory,
            IDataContextFactory<IdentityDataContext> identityDataContextFactory,
            IEnumerable<IDatabaseSeeder<DataContext>> databaseSeeders, IEnumerable<IDatabaseSeeder<IdentityDataContext>> identityDatabaseSeeders)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _identityDataContextFactory = identityDataContextFactory ?? throw new ArgumentNullException(nameof(identityDataContextFactory));
            _databaseSeeders = databaseSeeders ?? throw new ArgumentNullException(nameof(databaseSeeders));
            _identityDatabaseSeeders = identityDatabaseSeeders ?? throw new ArgumentNullException(nameof(identityDatabaseSeeders));
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

                 using (var context = _identityDataContextFactory.Create())
                {
                    try
                    {
                        context.Database.Migrate();
                        foreach (var seeder in _identityDatabaseSeeders)
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
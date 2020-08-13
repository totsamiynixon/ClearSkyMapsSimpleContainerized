using System;
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

        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly IDataContextFactory<IdentityDataContext> _identityDataContextFactory;
        private readonly IDatabaseSeeder<DataContext> _databaseSeeder;
        private readonly IDatabaseSeeder<IdentityDataContext> _identityDatabaseSeeder;

        public DefaultApplicationDatabaseInitializer(IDataContextFactory<DataContext> dataContextFactory,
            IDataContextFactory<IdentityDataContext> identityDataContextFactory,
            IDatabaseSeeder<DataContext> databaseSeeder, IDatabaseSeeder<IdentityDataContext> identityDatabaseSeeder)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _identityDataContextFactory = identityDataContextFactory ?? throw new ArgumentNullException(nameof(identityDataContextFactory));
            _databaseSeeder = databaseSeeder ?? throw new ArgumentNullException(nameof(databaseSeeder));
            _identityDatabaseSeeder = identityDatabaseSeeder ?? throw new ArgumentNullException(nameof(identityDatabaseSeeder));
        }


        public async Task InitializeDbAsync()
        {
            lock (Lock)
            {
                using (var context = _dataContextFactory.Create())
                {
                    try
                    {
                        context.Database.Migrate();
                        _databaseSeeder.SeedAsync(context).Wait();
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
                        _identityDatabaseSeeder.SeedAsync(context).Wait();
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
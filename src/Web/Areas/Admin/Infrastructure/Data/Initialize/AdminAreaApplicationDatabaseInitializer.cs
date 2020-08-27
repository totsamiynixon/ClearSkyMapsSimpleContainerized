using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.Data.Initialize.Seed;

namespace Web.Areas.Admin.Infrastructure.Data.Initialize
{
    public class AdminAreaApplicationDatabaseInitializer : IApplicationDatabaseInitializer
    {
        private static readonly object Lock = new object();

        protected readonly IDataContextFactory<IdentityDataContext> _identityDataContextFactory;
        protected readonly IEnumerable<IDatabaseSeeder<IdentityDataContext>> _identityDatabaseSeeders;

        public AdminAreaApplicationDatabaseInitializer(
            IDataContextFactory<IdentityDataContext> identityDataContextFactory,
            IEnumerable<IDatabaseSeeder<IdentityDataContext>> identityDatabaseSeeders)
        {
            _identityDataContextFactory = identityDataContextFactory ??
                                          throw new ArgumentNullException(nameof(identityDataContextFactory));
            _identityDatabaseSeeders = identityDatabaseSeeders ??
                                       throw new ArgumentNullException(nameof(identityDatabaseSeeders));
        }


        public virtual async Task InitializeDbAsync()
        {
            lock (Lock)
            {
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
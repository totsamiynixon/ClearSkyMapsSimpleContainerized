using Microsoft.EntityFrameworkCore.Design;

namespace Web.Areas.Admin.Infrastructure.Data.Factory
{
    public class DesignTimeIdentityDataContextFactory : EmulationDataContextFactory<IdentityDataContext>, IDesignTimeDbContextFactory<IdentityDataContext>
    {
        public static string ConnectionString { get; set; }
        
        public DesignTimeIdentityDataContextFactory() : base(ConnectionString)
        {
        }

        public IdentityDataContext CreateDbContext(string[] args)
        {
            return Create();
        }
    }
}
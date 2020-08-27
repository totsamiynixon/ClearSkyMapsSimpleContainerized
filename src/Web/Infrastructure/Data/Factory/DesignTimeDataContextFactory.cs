using Microsoft.EntityFrameworkCore.Design;

namespace Web.Infrastructure.Data.Factory
{
    public class DesignTimeDataContextFactory : DefaultDataContextFactory<DataContext>, IDesignTimeDbContextFactory<DataContext>
    {
        public static string ConnectionString { get; set; }
        
        public DesignTimeDataContextFactory() : base(ConnectionString)
        {
        }

        public DataContext CreateDbContext(string[] args)
        {
            return Create();
        }
    }
}
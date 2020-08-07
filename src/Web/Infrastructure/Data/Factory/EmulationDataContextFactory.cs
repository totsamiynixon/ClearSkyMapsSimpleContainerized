using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Factory
{
    public class EmulationDataContextFactory : IEmulationDataContextFactory<DataContext>
    {
        private readonly AppSettings _appSettings;
        public EmulationDataContextFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        
        public DataContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(_appSettings.Emulation.ConnectionString);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
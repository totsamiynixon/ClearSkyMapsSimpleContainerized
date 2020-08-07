using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Factory
{
    public class EmulationIdentityDataContextFactory : IEmulationDataContextFactory<IdentityDataContext>
    {
        private readonly AppSettings _appSettings;
        public EmulationIdentityDataContextFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        
        public IdentityDataContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDataContext>();
            optionsBuilder.UseSqlServer(_appSettings.ConnectionString);
            return new IdentityDataContext(optionsBuilder.Options);
        }
    }
}
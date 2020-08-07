using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Emulation;

namespace Web.Infrastructure.Data.Factory
{
    public class DefaultIdentityDataContextFactory : IDataContextFactory<IdentityDataContext>
    {
        private readonly AppSettings _appSettings;
        //TODO: Think more about how to swtich connections string in runtime
        private readonly Emulator _emulator;
        public DefaultIdentityDataContextFactory(AppSettings appSettings, Emulator emulator)
        {
            _appSettings = appSettings;
            _emulator = emulator;
        }
        
        public IdentityDataContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            //TODO: Think more about how to swtich connections string in runtime
            optionsBuilder.UseSqlServer(_emulator.IsEmulationEnabled ? _appSettings.Emulator.ConnectionString :_appSettings.ConnectionString);
            return new IdentityDataContext(optionsBuilder.Options);
        }
    }
}
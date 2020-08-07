using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Emulation;

namespace Web.Infrastructure.Data.Factory
{
    public class DefaultDataContextFactory : IDataContextFactory<DataContext>
    {
        private readonly AppSettings _appSettings;
        //TODO: Think more about how to swtich connections string in runtime
        private readonly Emulator _emulator;
        public DefaultDataContextFactory(AppSettings appSettings, Emulator emulator)
        {
            _appSettings = appSettings;
            _emulator = emulator;
        }
        
        public DataContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            //TODO: Think more about how to swtich connections string in runtime
            optionsBuilder.UseSqlServer(_emulator.IsEmulationEnabled ? _appSettings.Emulator.ConnectionString :_appSettings.ConnectionString);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
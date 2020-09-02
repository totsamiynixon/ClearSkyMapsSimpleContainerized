using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Infrastructure.Data.Factory
{
    public class EmulationDataContextFactory<TContext> : DefaultDataContextFactory<TContext>,
        IEmulationDataContextFactory<TContext> where TContext : DbContext
    {
        public EmulationDataContextFactory(string connectionString) : base(connectionString)
        {
        }
    }
}
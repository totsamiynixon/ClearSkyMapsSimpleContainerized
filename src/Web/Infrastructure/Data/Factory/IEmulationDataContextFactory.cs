using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Factory
{
    public interface IEmulationDataContextFactory<TContext> : IDataContextFactory<TContext> where TContext : DbContext
    {
    }
}
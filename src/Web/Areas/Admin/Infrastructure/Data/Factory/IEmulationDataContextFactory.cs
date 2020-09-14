using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Infrastructure.Data.Factory
{
    public interface IEmulationDataContextFactory<TContext> : IDataContextFactory<TContext> where TContext : DbContext
    {
    }
}
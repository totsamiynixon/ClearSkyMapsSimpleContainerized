using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Infrastructure.Data.Factory
{
    public interface IEmulationDataContextFactory<TContext> : IDataContextFactory<TContext> where TContext : DbContext
    {
    }
}
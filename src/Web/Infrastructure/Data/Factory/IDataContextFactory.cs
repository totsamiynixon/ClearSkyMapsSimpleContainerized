using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Factory
{
    public interface IDataContextFactory<TContext> where TContext : DbContext
    {
        public TContext Create();
      
    }
}
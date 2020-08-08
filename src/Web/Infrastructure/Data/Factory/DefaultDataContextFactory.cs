using System;
using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Factory
{
    public class DefaultDataContextFactory<TContext> : IEmulationDataContextFactory<TContext> where TContext : DbContext

    {
        private readonly string _connectionString;

        public DefaultDataContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }


        public TContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            /*var optionsType = typeof(DbContextOptions<>).MakeGenericType(typeof(TContext));
            var options = (DbContextOptions)Activator.CreateInstance(optionsType);*/
            return (TContext) Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);
        }
    }
}
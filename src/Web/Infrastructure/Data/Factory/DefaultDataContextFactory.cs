using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Web.Areas.Admin.Infrastructure.Data.Factory;

namespace Web.Infrastructure.Data.Factory
{
    public class DefaultDataContextFactory<TContext> : IDataContextFactory<TContext> where TContext : DbContext

    {
        private readonly string _connectionString;

        public DefaultDataContextFactory(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }
            
            _connectionString = connectionString;
        }


        public TContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            return (TContext) Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);
        }
    }
}
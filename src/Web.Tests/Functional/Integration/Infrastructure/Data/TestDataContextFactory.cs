using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Data.Factory;

namespace Web.Tests.Functional.Integration.Infrastructure.Data
{
    public class TestDataContextFactory<TContext> : IEmulationDataContextFactory<TContext> where TContext : DbContext
    {
        private readonly Dictionary<String, DbConnection> _connections = new Dictionary<string, DbConnection>();
        
        public TContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            string connectionName = typeof(TContext).Name;
            DbConnection connection = null;
            if (_connections.ContainsKey(connectionName))
            {
                connection = _connections[connectionName];
            }
            else
            { 
                connection = CreateInMemoryDatabase(connectionName);
                _connections.Add(connectionName, connection);
            }

            optionsBuilder.UseSqlite(connection);
            return (TContext) Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);
        }
        
        private static DbConnection CreateInMemoryDatabase(string name)
        {
            var connection = new SqliteConnection($"DataSource={name};mode=memory;cache=shared");

            connection.Open();
        
            return connection;
        }
    }
}
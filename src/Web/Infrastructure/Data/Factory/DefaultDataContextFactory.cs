﻿using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Factory
{
    public class DefaultDataContextFactory : IDataContextFactory<DataContext>
    {
        private readonly AppSettings _appSettings;

        public DefaultDataContextFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }


        public DataContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(_appSettings.ConnectionString);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
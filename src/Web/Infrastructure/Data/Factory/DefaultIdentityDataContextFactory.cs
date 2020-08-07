﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Data.Factory
{
    public class DefaultIdentityDataContextFactory : IDataContextFactory<IdentityDataContext>
    {
        private readonly AppSettings _appSettings;

        public DefaultIdentityDataContextFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IdentityDataContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDataContext>();
            optionsBuilder.UseSqlServer(_appSettings.ConnectionString);
            return new IdentityDataContext(optionsBuilder.Options);
        }
    }
}
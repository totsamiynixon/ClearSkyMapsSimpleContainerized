using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.Models.Identity;
using Web.Helpers.Interfaces;

namespace Web.Middlewares
{
    public static class DatabaseSeederMiddleware
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var repository = serviceScope.ServiceProvider.GetService<IRepository>();
                repository.ReinitializeDb();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Data;

namespace ProductConfigurator.Tests
{
    public class ApiFactory : WebApplicationFactory<Program>
    {
        private const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=ProductConfigurator_Tests;Trusted_Connection=True";

        public ApiFactory()
        {
            // A fresh test database with the current schema, before the app starts
            var options = new DbContextOptionsBuilder<ConfiguratorDbContext>().UseSqlServer(ConnectionString).Options;
            using var db = new ConfiguratorDbContext(options);
            db.Database.EnsureDeleted();
            db.Database.Migrate();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Point the app at the test database instead of your dev database
            builder.UseSetting("ConnectionStrings:Default", ConnectionString);
        }
    }
}

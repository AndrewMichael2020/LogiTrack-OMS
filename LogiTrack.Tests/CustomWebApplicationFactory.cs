using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LogiTrack.Models;
using System.Linq;
using System.Collections.Generic;

namespace LogiTrack.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing context registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<LogiTrackContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Set environment variable for test DB
                Environment.SetEnvironmentVariable("ASPNETCORE_TEST_DB", "TestLogiTrack.db");

                services.AddDbContext<LogiTrackContext>(options =>
                {
                    options.UseSqlite("Data Source=TestLogiTrack.db");
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<LogiTrackContext>();
                    db.Database.EnsureDeleted();
                    db.Database.Migrate();
                }
            })
            .ConfigureAppConfiguration((context, configBuilder) =>
            {
                var testConfig = new Dictionary<string, string>
                {
                    { "Jwt:Key", "supersecretkey1234supersecretkey1234" }, // 32 chars, 256 bits
                    { "Jwt:Issuer", "logitrack-test" },
                    { "Jwt:Audience", "logitrack-test" }
                };
                configBuilder.AddInMemoryCollection(testConfig);
            });
        }
    }
}

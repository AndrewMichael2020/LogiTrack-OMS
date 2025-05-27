using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace LogiTrack.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Do NOT register or remove DbContext here.
                // Only use the context to ensure database is created and seed roles.

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LogiTrackContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

                    db.Database.EnsureCreated();

                    // Only call Migrate if using a relational provider
                    var relational = db.Database.ProviderName != null &&
                                     db.Database.IsRelational();
                    if (relational)
                    {
                        db.Database.Migrate();
                    }

                    // Seed roles if needed
                    if (!roleManager.Roles.Any(r => r.Name == "Manager"))
                    {
                        roleManager.CreateAsync(new IdentityRole("Manager")).Wait();
                    }
                    if (!roleManager.Roles.Any(r => r.Name == "User"))
                    {
                        roleManager.CreateAsync(new IdentityRole("User")).Wait();
                    }
                }
            });
        }
    }
}

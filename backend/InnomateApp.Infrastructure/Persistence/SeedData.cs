using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Security;
using InnomateApp.Application.Interfaces;
namespace InnomateApp.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            // ✅ Seed Roles
            if (!context.Roles.Any())
            {
                var adminRole = new Role { Name = "Admin" };
                context.Roles.Add(adminRole);
                context.SaveChanges();
            }

            // ✅ Seed Admin User
            if (!context.Users.IgnoreQueryFilters().Any())
            {
                var adminUser = User.Create(1, "admin", "admin@example.com", passwordHasher.HashPassword("Admin@123"));

                var adminRole = context.Roles.First(r => r.Name == "Admin");
                adminUser.Roles = new List<Role> { adminRole };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

        }
    }
}
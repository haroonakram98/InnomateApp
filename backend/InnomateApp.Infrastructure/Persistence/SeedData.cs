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

                // ✅ Seed Permissions (add more as needed)
                var permissions = new[]
                {
                new Permission { Name = "Manage Users", Code = "USERS_MANAGE" },
                new Permission { Name = "View Reports", Code = "REPORTS_VIEW" }
            };
                context.Permissions.AddRange(permissions);
                context.SaveChanges();

                // ✅ Assign all permissions to Admin
                adminRole.Permissions = permissions.ToList();
                context.SaveChanges();
            }

            // ✅ Seed Admin User
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"), // Set default
                    CreatedAt = DateTime.UtcNow
                };

                var adminRole = context.Roles.First(r => r.Name == "Admin");
                adminUser.Roles = new List<Role> { adminRole };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
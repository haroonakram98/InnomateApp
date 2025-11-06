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
                    new Permission { Name = "View Reports", Code = "REPORTS_VIEW" },
                    new Permission { Name = "EditUsers", Code = "USERS_EDIT" }
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

            if (!context.Customers.Any())
            {
                var customers = new List<Customer>
                {
                    new Customer
                    {
                        Name = "Ali Khan",
                        Phone = "0300-1234567",
                        Email = "ali.khan@example.com",
                        Address = "House #24, Street 10, G-11/3, Islamabad"
                    },
                    new Customer
                    {
                        Name = "Sara Ahmed",
                        Phone = "0321-9988776",
                        Email = "sara.ahmed@example.com",
                        Address = "Flat #5B, Gulshan-e-Iqbal, Karachi"
                    },
                    new Customer
                    {
                        Name = "Usman Raza",
                        Phone = "0333-5556677",
                        Email = "usman.raza@example.com",
                        Address = "Street 8, Cavalry Ground, Lahore"
                    },
                    new Customer
                    {
                        Name = "Hina Malik",
                        Phone = "0312-4445566",
                        Email = "hina.malik@example.com",
                        Address = "Sector F-10, Islamabad"
                    },
                    new Customer
                    {
                        Name = "Bilal Sheikh",
                        Phone = "0345-7788990",
                        Email = "bilal.sheikh@example.com",
                        Address = "Model Town, Lahore"
                    }
                };

                context.Customers.AddRangeAsync(customers);
                context.SaveChangesAsync();
            }
        }
    }
}
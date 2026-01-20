using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InnomateApp.Domain.Entities;
using InnomateApp.Application.Interfaces;
using InnomateApp.Infrastructure.Persistence;

namespace InnomateApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await Task.CompletedTask;
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
            return user;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await Task.CompletedTask;
            return true;
        }

        public async Task UpdateLastLoginAsync(int userId, DateTime timestamp)
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync( user => user.UserId == userId);
            if (user == null) return;

            user.LastLoginAt = timestamp;
            await Task.CompletedTask;
        }
        public async Task<User?> GetUserByIdIgnoreFilterAsync(int id)
        {
            return await _context.Users
                .IgnoreQueryFilters()  // This will ignore all query filters
                .FirstOrDefaultAsync(u => u.UserId == id);
        }
    }
}

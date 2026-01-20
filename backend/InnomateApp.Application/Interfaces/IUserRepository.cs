using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(User user);
    Task UpdateLastLoginAsync(int userId, DateTime timestamp);
    Task<User?> GetUserByIdIgnoreFilterAsync(int id);
}
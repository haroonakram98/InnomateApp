using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<Role> AddAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task<bool> DeleteAsync(int id);
}

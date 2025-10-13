using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Permissions
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
}

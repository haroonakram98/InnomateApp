using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.RoleId == id);
    }

    public async Task<Role> AddAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        // The caller should have loaded the existing role (or pass the tracked entity).
        // To be safe here, attach if necessary:
        var tracked = await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.RoleId == role.RoleId);

        if (tracked == null)
            throw new InvalidOperationException($"Role with id {role.RoleId} not found.");

        // update scalar properties
        tracked.Name = role.Name;

        // update many-to-many: replace tracked.Permissions with incoming Permission entities
        tracked.Permissions.Clear();
        foreach (var p in role.Permissions)
        {
            // Attach permission entity if not tracked
            var perm = await _context.Permissions.FindAsync(p.PermissionId);
            if (perm != null)
            {
                tracked.Permissions.Add(perm);
            }
        }

        await _context.SaveChangesAsync();
        return tracked;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Roles.FindAsync(id);
        if (existing == null) return false;
        _context.Roles.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

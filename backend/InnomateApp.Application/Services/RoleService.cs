using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepo;
    private readonly IPermissionRepository _permissionRepo;

    public RoleService(IRoleRepository roleRepo, IPermissionRepository permissionRepo)
    {
        _roleRepo = roleRepo;
        _permissionRepo = permissionRepo;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepo.GetAllAsync();
        return roles.Select(r => MapToDto(r));
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _roleRepo.GetByIdAsync(id);
        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        var permissions = (await _permissionRepo.GetByIdsAsync(dto.PermissionIds)).ToList();

        var role = new Role
        {
            Name = dto.Name,
            Permissions = permissions
        };

        var created = await _roleRepo.AddAsync(role);
        return MapToDto(created);
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto dto)
    {
        var existing = await _roleRepo.GetByIdAsync(id);
        if (existing == null) return null;

        var permissions = (await _permissionRepo.GetByIdsAsync(dto.PermissionIds)).ToList();

        existing.Name = dto.Name;
        existing.Permissions = permissions;

        var updated = await _roleRepo.UpdateAsync(existing);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        return await _roleRepo.DeleteAsync(id);
    }

    // mapping helpers
    private static RoleDto MapToDto(Role r)
    {
        var perms = r.Permissions?.Select(p => new PermissionDto(p.PermissionId, p.Name, p.Code)) ?? Enumerable.Empty<PermissionDto>();
        return new RoleDto(r.RoleId, r.Name, perms);
    }
}

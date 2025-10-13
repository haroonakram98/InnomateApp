namespace InnomateApp.Application.DTOs;

public record PermissionDto(int Id, string Name, string Code);

public record RoleDto(int Id, string Name, IEnumerable<PermissionDto> Permissions);

public record CreateRoleDto(string Name, IEnumerable<int> PermissionIds);

public record UpdateRoleDto(string Name, IEnumerable<int> PermissionIds);
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities;

public class Permission
{
    [Key]
    public int PermissionId { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;  // e.g., "CanEditUsers"
    public string Code { get; set; } = string.Empty;  // Unique key, e.g., "EDIT_USERS"

    // Navigation
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
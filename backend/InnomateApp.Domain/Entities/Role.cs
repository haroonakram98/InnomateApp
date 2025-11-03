using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities;

public class Role
{
    [Key]
    public int RoleId { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty; // e.g., Admin, Manager, User

    // Navigation
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities;

public class User
{
    [Key]
    public int UserId { get; set; }
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public DateTime? LastLoginAt { get; set; }
}
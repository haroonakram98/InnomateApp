using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities;

public class User : TenantEntity
{
    [Key]
    public int UserId { get; set; }
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public DateTime? LastLoginAt { get; set; }

    public User() { }

    public static User Create(int tenantId, string username, string email, string passwordHash)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.Now
        };
        user.SetTenantId(tenantId);
        return user;
    }
}
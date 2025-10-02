namespace InnomateApp.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;  // e.g., "CanEditUsers"
    public string Code { get; set; } = string.Empty;  // Unique key, e.g., "EDIT_USERS"

    // Navigation
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
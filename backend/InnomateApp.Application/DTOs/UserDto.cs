
namespace InnomateApp.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;   // map from your User entity
        public string UserName { get; set; } = string.Empty; // previously Username
        public string Email { get; set; } = string.Empty;
        public int TenantId { get; set; }
    }
}
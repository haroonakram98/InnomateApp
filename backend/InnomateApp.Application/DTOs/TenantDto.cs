namespace InnomateApp.Application.DTOs
{
    public class TenantOnboardingDto
    {
        public string TenantName { get; set; } = string.Empty;
        public string TenantCode { get; set; } = string.Empty;
        
        // Initial Admin User
        public string AdminUsername { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
    }

    public class TenantResponseDto
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string AdminUsername { get; set; } = string.Empty;
    }
}

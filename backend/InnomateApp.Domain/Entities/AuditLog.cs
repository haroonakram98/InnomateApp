namespace InnomateApp.Domain.Entities;

public class AuditLog
{
	public int Id { get; set; }
	public string Action { get; set; } = string.Empty;   // e.g., "User Login"
	public string PerformedBy { get; set; } = string.Empty; // Username/Email
	public DateTime PerformedAt { get; set; } = DateTime.Now;
	public string Details { get; set; } = string.Empty;  // JSON / description
}
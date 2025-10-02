using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IAuditLogger
    {
        Task LogAsync(string action, string performedBy, string details = "");
    }
}
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
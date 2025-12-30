using InnomateApp.Application.Interfaces;
using System.Security.Claims;

namespace InnomateApp.API.Context
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetTenantId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return 0;

            var claim = user.FindFirst("tenant_id");
            if (claim != null && int.TryParse(claim.Value, out var tenantId))
            {
                return tenantId;
            }

            return 0;
        }

        public int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return 0;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out var userId))
            {
                return userId;
            }

            return 0;
        }
    }
}

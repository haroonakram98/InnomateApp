using InnomateApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InnomateApp.Infrastructure.Identity
{
    public class HttpTenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int? _overriddenTenantId;

        public HttpTenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public int GetTenantId()
        {
            if (_overriddenTenantId.HasValue)
                return _overriddenTenantId.Value;

            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated != true)
                return 0;

            var claim = _httpContextAccessor.HttpContext.User.FindFirst("tenant_id");
            return claim != null && int.TryParse(claim.Value, out var tenantId) ? tenantId : 0;
        }

        public int GetUserId()
        {
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated != true)
                return 0;

            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
        }

        public void SetTenantIdForBackgroundOperation(int tenantId)
        {
            _overriddenTenantId = tenantId;
        }

        public void ResetTenantId()
        {
            _overriddenTenantId = null;
        }
    }
}
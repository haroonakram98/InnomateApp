namespace InnomateApp.Application.Interfaces
{
    /// <summary>
    /// Provides access to the current tenant ID from the request context.
    /// Used by DbContext for global query filters and automatic tenant stamping.
    /// </summary>
    public interface ITenantProvider
    {
        /// <summary>
        /// Gets the current TenantId. Returns 0 if no tenant is found (e.g., during background tasks or public requests).
        /// </summary>
        int GetTenantId();

        /// <summary>
        /// Gets the current UserId for auditing purposes.
        /// </summary>
        int GetUserId();
    }
}

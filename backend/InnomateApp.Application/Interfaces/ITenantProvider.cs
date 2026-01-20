namespace InnomateApp.Application.Interfaces
{
    /// <summary>
    /// Provides access to the current tenant and user context.
    /// </summary>
    public interface ITenantProvider
    {
        /// <summary>
        /// Gets the current tenant ID. Returns 0 if no tenant is found.
        /// For HTTP contexts, this is typically set from the 'tenant_id' claim.
        /// </summary>
        int GetTenantId();

        /// <summary>
        /// Gets the current user ID for auditing purposes.
        /// For HTTP contexts, this is typically set from the 'sub' or NameIdentifier claim.
        /// </summary>
        int GetUserId();

        void SetTenantIdForBackgroundOperation(int tenantId);
        void ResetTenantId();
    }
}
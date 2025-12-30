namespace InnomateApp.Domain.Common
{
    /// <summary>
    /// Base entity for multi-tenant support
    /// All tenant-specific entities should inherit from this
    /// </summary>
    public abstract class TenantEntity
    {
        /// <summary>
        /// Tenant identifier for data isolation
        /// </summary>
        public int TenantId { get; protected set; }

        public virtual void SetTenantId(int tenantId)
        {
            if (tenantId <= 0)
                throw new ArgumentException("TenantId must be greater than zero", nameof(tenantId));
            
            if (TenantId != 0 && TenantId != tenantId)
                throw new InvalidOperationException("TenantId is already set and cannot be changed.");

            TenantId = tenantId;
        }
    }
}

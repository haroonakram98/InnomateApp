namespace InnomateApp.Domain.Common
{
    /// <summary>
    /// Base class for all domain exceptions
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        
        protected DomainException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when business rule is violated
    /// </summary>
    public class BusinessRuleViolationException : DomainException
    {
        public BusinessRuleViolationException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when insufficient stock for operation
    /// </summary>
    public class InsufficientStockException : DomainException
    {
        public int ProductId { get; }
        public decimal Required { get; }
        public decimal Available { get; }

        public InsufficientStockException(int productId, decimal required, decimal available)
            : base($"Insufficient stock for product {productId}. Required: {required}, Available: {available}")
        {
            ProductId = productId;
            Required = required;
            Available = available;
        }
    }

    /// <summary>
    /// Thrown when entity is not found
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public string EntityName { get; }
        public object EntityId { get; }

        public EntityNotFoundException(string entityName, object entityId)
            : base($"{entityName} with ID {entityId} was not found")
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }

    /// <summary>
    /// Thrown when trying to operate on inactive entity
    /// </summary>
    public class InactiveEntityException : DomainException
    {
        public InactiveEntityException(string entityName, object entityId)
            : base($"{entityName} with ID {entityId} is inactive") { }
    }
}

using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Categories.Commands.DeleteCategory
{
    /// <summary>
    /// Handler for DeleteCategoryCommand
    /// Validates business rules before deletion
    /// </summary>
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<DeleteCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();

            // Fetch existing category
            var category = await _unitOfWork.Categories.GetByIdAsync(command.CategoryId);
            if (category == null || category.TenantId != tenantId)
            {
                return Result<bool>.NotFound($"Category with ID {command.CategoryId} not found");
            }

            // Check if category has products
            var productCount = await _unitOfWork.Categories.GetProductCountAsync(command.CategoryId);
            if (productCount > 0)
            {
                return Result<bool>.Failure(
                    $"Cannot delete category '{category.Name}' because it has {productCount} associated product(s). " +
                    "Please reassign or delete the products first.");
            }

            await _unitOfWork.Categories.DeleteAsync(category);
            // Note: SaveChangesAsync is called by EnhancedTransactionBehavior

            _logger.LogInformation("Category '{CategoryName}' deleted (ID: {CategoryId})", 
                category.Name, category.CategoryId);

            return Result<bool>.Success(true);
        }
    }
}

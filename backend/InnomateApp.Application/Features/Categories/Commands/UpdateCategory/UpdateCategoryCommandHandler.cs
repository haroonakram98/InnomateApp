using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Categories.Commands.UpdateCategory
{
    /// <summary>
    /// Handler for UpdateCategoryCommand
    /// </summary>
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<UpdateCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<CategoryResponse>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var tenantId = _tenantProvider.GetTenantId();

            // Fetch existing category
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null || category.TenantId != tenantId)
            {
                return Result<CategoryResponse>.NotFound($"Category with ID {request.CategoryId} not found");
            }

            // Check for duplicate name (excluding current category)
            var duplicateCategory = await _unitOfWork.Categories.GetByNameAsync(request.Name.Trim(), tenantId);
            if (duplicateCategory != null && duplicateCategory.CategoryId != request.CategoryId)
            {
                return Result<CategoryResponse>.Failure($"Category '{request.Name}' already exists");
            }

            // Update entity using domain method
            category.Update(request.Name, request.Description);

            await _unitOfWork.Categories.UpdateAsync(category);
            // Note: SaveChangesAsync is called by EnhancedTransactionBehavior

            _logger.LogInformation("Category '{CategoryName}' updated (ID: {CategoryId})", 
                category.Name, category.CategoryId);

            var productCount = await _unitOfWork.Categories.GetProductCountAsync(category.CategoryId);

            return Result<CategoryResponse>.Success(new CategoryResponse
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ProductCount = productCount
            });
        }
    }
}

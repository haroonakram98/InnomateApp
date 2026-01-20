using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Common;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Categories.Commands.CreateCategory
{
    /// <summary>
    /// Handler for CreateCategoryCommand
    /// Validates business rules and creates the category entity
    /// </summary>
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<CreateCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var tenantId = _tenantProvider.GetTenantId();

            // Check for duplicate name within tenant
            var existingCategory = await _unitOfWork.Categories.GetByNameAsync(request.Name.Trim(), tenantId);
            if (existingCategory != null)
            {
                return Result<CategoryResponse>.Failure($"Category '{request.Name}' already exists");
            }

            // Use factory method for domain entity creation
            var category = Category.Create(tenantId, request.Name, request.Description);

            await _unitOfWork.Categories.AddAsync(category);
            // Note: SaveChangesAsync is called by EnhancedTransactionBehavior

            _logger.LogInformation("Category '{CategoryName}' created with ID: {CategoryId}", 
                category.Name, category.CategoryId);

            return Result<CategoryResponse>.Success(new CategoryResponse
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ProductCount = 0
            });
        }
    }
}

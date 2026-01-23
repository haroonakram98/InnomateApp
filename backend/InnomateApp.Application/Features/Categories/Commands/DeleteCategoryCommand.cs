using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }

        public DeleteCategoryCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(IUnitOfWork uow, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _uow.Categories.GetByIdAsync(request.Id);
                if (category == null)
                    return Result<bool>.Failure("Category not found.");

                // Check for products in this category before deleting
                // This is a business rule: Can't delete category if it has products.
                // Assuming _uow.Categories has some way to check this or we use ProductRepository
                
                // For now, let's just delete and let the DB constraint handle it or check products
                // Actually, I'll check it to provide a better error message.
                var products = await _uow.Products.GetAllAsync();
                if (products.Any(p => p.CategoryId == request.Id))
                {
                    return Result<bool>.Failure("Cannot delete category because it has associated products.");
                }

                await _uow.Categories.DeleteAsync(category);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category deleted successfully: {CategoryId}", request.Id);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", request.Id);
                return Result<bool>.Failure("An unexpected error occurred while deleting the category.");
            }
        }
    }
}

using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Categories.Commands
{
    public class ToggleCategoryStatusCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }

        public ToggleCategoryStatusCommand(int id)
        {
            Id = id;
        }
    }

    public class ToggleCategoryStatusCommandHandler : IRequestHandler<ToggleCategoryStatusCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ToggleCategoryStatusCommandHandler> _logger;

        public ToggleCategoryStatusCommandHandler(IUnitOfWork uow, ILogger<ToggleCategoryStatusCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ToggleCategoryStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _uow.Categories.GetByIdAsync(request.Id);
                if (category == null)
                    return Result<bool>.Failure("Category not found.");

                category.ToggleStatus();
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category {CategoryId} status toggled to {IsActive}", 
                    category.CategoryId, category.IsActive);

                return Result<bool>.Success(category.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category status {CategoryId}", request.Id);
                return Result<bool>.Failure("An unexpected error occurred.");
            }
        }
    }
}

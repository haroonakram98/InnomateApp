using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Requests;
using InnomateApp.Application.DTOs.Categories.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Categories.Commands.CreateCategory
{
    /// <summary>
    /// Command for creating a new category
    /// Transaction will be handled by EnhancedTransactionBehavior
    /// </summary>
    public class CreateCategoryCommand : IRequest<Result<CategoryResponse>>
    {
        public CreateCategoryRequest Request { get; }

        public CreateCategoryCommand(CreateCategoryRequest request)
        {
            Request = request;
        }
    }
}

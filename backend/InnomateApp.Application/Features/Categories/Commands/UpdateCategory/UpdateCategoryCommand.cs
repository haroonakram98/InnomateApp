using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Requests;
using InnomateApp.Application.DTOs.Categories.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Categories.Commands.UpdateCategory
{
    /// <summary>
    /// Command for updating an existing category
    /// </summary>
    public class UpdateCategoryCommand : IRequest<Result<CategoryResponse>>
    {
        public UpdateCategoryRequest Request { get; }

        public UpdateCategoryCommand(UpdateCategoryRequest request)
        {
            Request = request;
        }
    }
}

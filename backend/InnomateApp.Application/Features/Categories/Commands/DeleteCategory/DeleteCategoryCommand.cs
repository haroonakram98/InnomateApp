using InnomateApp.Application.Common;
using MediatR;

namespace InnomateApp.Application.Features.Categories.Commands.DeleteCategory
{
    /// <summary>
    /// Command for deleting a category
    /// </summary>
    public class DeleteCategoryCommand : IRequest<Result<bool>>
    {
        public int CategoryId { get; }

        public DeleteCategoryCommand(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}

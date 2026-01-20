using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Categories.Queries.GetCategoryById
{
    /// <summary>
    /// Query for retrieving a category by ID
    /// </summary>
    [NoTransaction]
    public class GetCategoryByIdQuery : IRequest<Result<CategoryResponse>>, IBaseQuery
    {
        public int CategoryId { get; }

        public GetCategoryByIdQuery(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}

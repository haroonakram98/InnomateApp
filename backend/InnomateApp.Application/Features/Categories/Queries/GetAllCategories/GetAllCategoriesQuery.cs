using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Categories.Queries.GetAllCategories
{
    /// <summary>
    /// Query for retrieving all categories
    /// Marked with NoTransaction as reads don't require transactions
    /// </summary>
    [NoTransaction]
    public class GetAllCategoriesQuery : IRequest<List<CategoryResponse>>, IBaseQuery
    {
    }
}

using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Categories.Queries.GetCategoryLookup
{
    /// <summary>
    /// Query for retrieving lightweight category list for dropdowns
    /// </summary>
    [NoTransaction]
    public class GetCategoryLookupQuery : IRequest<List<CategoryLookupResponse>>, IBaseQuery
    {
    }
}

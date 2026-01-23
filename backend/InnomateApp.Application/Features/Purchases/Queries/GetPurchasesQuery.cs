using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Queries
{
    public class GetPurchasesQuery : IRequest<Result<IEnumerable<PurchaseResponseDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; }
    }

    public class GetPurchasesQueryHandler : IRequestHandler<GetPurchasesQuery, Result<IEnumerable<PurchaseResponseDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetPurchasesQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<PurchaseResponseDto>>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate ?? DateTime.UtcNow.AddMonths(-1);
            var endDate = request.EndDate ?? DateTime.UtcNow;
            
            // Ensure end date includes the entire day
            var endDateInclusive = endDate.Date.AddDays(1).AddSeconds(-1);

            var purchases = await _uow.Purchases.GetPurchasesByDateRangeAsync(startDate, endDateInclusive);
            
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                purchases = purchases.Where(p => 
                    p.InvoiceNo.ToLower().Contains(search) || 
                    (p.Supplier != null && p.Supplier.Name.ToLower().Contains(search))
                ).ToList();
            }

            var dtos = _mapper.Map<IEnumerable<PurchaseResponseDto>>(purchases);
            return Result<IEnumerable<PurchaseResponseDto>>.Success(dtos);
        }
    }
}

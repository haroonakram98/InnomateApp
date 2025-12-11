using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetAvailableBatchesQuery : IRequest<Result<List<FifoBatchDto>>>
    {
        public int ProductId { get; set; }
    }

    public class GetAvailableBatchesQueryHandler : IRequestHandler<GetAvailableBatchesQuery, Result<List<FifoBatchDto>>>
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        public GetAvailableBatchesQueryHandler(IStockRepository stockRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<FifoBatchDto>>> Handle(GetAvailableBatchesQuery request, CancellationToken cancellationToken)
        {
            var batches = await _stockRepository.GetAvailableBatchesForProductAsync(request.ProductId);
            var batchDtos = _mapper.Map<List<FifoBatchDto>>(batches);

            return Result<List<FifoBatchDto>>.Success(batchDtos);
        }
    }
}

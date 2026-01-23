using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Features.Customers.Queries
{
    public class GetCustomersQuery : IRequest<Result<IEnumerable<CustomerDto>>>
    {
        public string? Search { get; set; }
    }

    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<IEnumerable<CustomerDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCustomersQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Customer> customers = await _uow.Customers.GetAllAsync();
            
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                customers = customers.Where(c => 
                    c.Name.ToLower().Contains(search) || 
                    (c.Email != null && c.Email.ToLower().Contains(search)) ||
                    (c.Phone != null && c.Phone.Contains(search))
                );
            }

            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Result<IEnumerable<CustomerDto>>.Success(customerDtos);
        }
    }
}

using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Commands.CreateSale
{
    public class CreateSaleCommand : IRequest<Result<int>>
    {
        public CreateSaleDto SaleDto { get; set; } = null!;
    }
}

using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.DTOs.Sales.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Sales.Commands.CreateSale
{
    public class CreateSaleCommand : IRequest<Result<int>>
    {
        public CreateSaleRequest SaleDto { get; set; } = null!;
    }
}

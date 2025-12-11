using InnomateApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public TransactionBehavior(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Check if the request needs transaction (you can use attributes or interfaces)
            if (!RequiresTransaction(request))
            {
                return await next();
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var response = await next();
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private bool RequiresTransaction(TRequest request)
        {
            // Implement logic to determine if this request needs a transaction
            // You can use attributes, marker interfaces, or naming conventions
            return request.GetType().Name.EndsWith("Command");
        }
    }
}

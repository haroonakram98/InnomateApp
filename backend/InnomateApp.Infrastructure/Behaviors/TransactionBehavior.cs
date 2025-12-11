using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Skip transaction for queries
            if (IsQuery(request))
            {
                return await next();
            }

            _logger.LogInformation("Beginning transaction for {RequestType}", typeof(TRequest).Name);

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var response = await next();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync();

                _logger.LogInformation("Transaction committed for {RequestType}", typeof(TRequest).Name);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during transaction for {RequestType}. Rolling back.", typeof(TRequest).Name);
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static bool IsQuery(TRequest request)
        {
            return request.GetType().Name.EndsWith("Query");
        }
        
    }
    // Marker interface for queries
    public interface IBaseQuery { }

}
using Azure;
using Azure.Core;
using InnomateApp.Application.Attributes;
using InnomateApp.Application.Interfaces;
using InnomateApp.Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Behaviors
{
    public class EnhancedTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EnhancedTransactionBehavior<TRequest, TResponse>> _logger;

        public EnhancedTransactionBehavior(IUnitOfWork unitOfWork, ILogger<EnhancedTransactionBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Check for attributes
            if (request.GetType().GetCustomAttributes(typeof(NoTransactionAttribute), false).Any())
            {
                return await next();
            }

            if (request.GetType().GetCustomAttributes(typeof(ManualTransactionAttribute), false).Any())
            {
                // Let the handler manage its own transaction
                return await next();
            }

            // Skip transaction for queries
            if (IsQuery(request))
            {
                return await next();
            }

            return await ExecuteInTransactionAsync(request, next, cancellationToken);
        }

        private async Task<TResponse> ExecuteInTransactionAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
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
            return request.GetType().Name.EndsWith("Query") ||
                   request is IBaseQuery;
        }
    }
}
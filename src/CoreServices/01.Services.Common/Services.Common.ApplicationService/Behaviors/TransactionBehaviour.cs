using MediatR;
using Microsoft.Extensions.Logging;
using Services.Common.ApplicationService.Extensions;
using Services.Common.Domain.Uow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Common.ApplicationService.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse>> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_unitOfWork.HasActiveTransaction)
                {
                    return await next();
                }

                TResponse response;

                using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
                {
                    _logger.LogInformation("-----Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                    response = await next();

                    _logger.LogInformation("-----Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                    await transaction.CommitAsync(cancellationToken);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--- ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
using Ardalis.GuardClauses;
using SagaPattern.Commons;
using Shipping.Api.Interfaces;

namespace Shipping.Api.Commands;

public class DeleteShippingRequestCommandHandler : ICommandHandler<DeleteShippingRequestCommand>
{
    private readonly IShippingRequestRepository _shippingRequestRepository;
    private readonly ILogger<DeleteShippingRequestCommandHandler> _logger;

    public DeleteShippingRequestCommandHandler(
            IShippingRequestRepository shippingRequestRepository, 
            ILogger<DeleteShippingRequestCommandHandler> logger
        )
    {
        _shippingRequestRepository = shippingRequestRepository;
        _logger = logger;
    }
    public async Task HandleAsync(DeleteShippingRequestCommand command)
    {
        Guard.Against.Null(command);

        await _shippingRequestRepository.DeleteShippingRequestAsync(command.ShippingRequestId);
        _logger.LogInformation($"deleted shipping request with id: {command.ShippingRequestId} from db");
    }
}
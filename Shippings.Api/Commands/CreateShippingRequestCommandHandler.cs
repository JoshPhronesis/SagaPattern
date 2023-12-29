using Ardalis.GuardClauses;
using SagaPattern.Commons;
using Shipping.Api.Events;
using Shipping.Api.Interfaces;

namespace Shipping.Api.Commands;

public class CreateShippingRequestCommandHandler : ICommandHandler<CreateShippingRequestCommand>
{
    private readonly IShippingRequestRepository _repository;
    private readonly ISqsMessenger _sqsMessenger;
    private readonly ILogger<CreateShippingRequestCommandHandler> _logger;

    public CreateShippingRequestCommandHandler(
        IShippingRequestRepository repository, 
        ISqsMessenger sqsMessenger,
        ILogger<CreateShippingRequestCommandHandler> logger)
    {
        _repository = repository;
        _sqsMessenger = sqsMessenger;
        _logger = logger;
    }
    public async Task HandleAsync(CreateShippingRequestCommand command)
    {
        Guard.Against.Null(command);

        try
        {
            await _repository.CreateShippingRequestAsync(command.ShippingRequest);
            _logger.LogInformation($"inserted shipping request with id: {command.ShippingRequest.Id} into db");
        }
        catch
        {
            await _sqsMessenger.SendMessageAsync(new ShippingCancelledEvent()
            {
                OrderId = command.ShippingRequest.OrderId,
                PaymentDetailsId = command.ShippingRequest.PaymentId
            });
        }
    }
}
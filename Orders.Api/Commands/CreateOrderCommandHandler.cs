using Ardalis.GuardClauses;
using SagaPattern.Commons;
using OrdersService.Data;
using OrdersService.Events;

namespace OrdersService.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly ISqsMessenger _sqsMessenger;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IOrdersRepository ordersRepository, ISqsMessenger sqsMessenger, ILogger<CreateOrderCommandHandler> logger)
    {
        _ordersRepository = ordersRepository;
        _sqsMessenger = sqsMessenger;
        _logger = logger;
    }
    public async Task HandleAsync(CreateOrderCommand command)
    {
        Guard.Against.Null(command);
        command.Order.Id = Guid.NewGuid();

        await _ordersRepository.CreateOrderAsync(command.Order);
        _logger.LogInformation($"inserted order with id: {command.Order.Id} into db");
        // persisting data to our event store
        await _sqsMessenger.SendMessageAsync(new OrderCreatedEvent
        {
            Currency = command.Order.Currency,
            Amount = command.Order.Amount,
            OrderId = command.Order.Id
        });
    }
}
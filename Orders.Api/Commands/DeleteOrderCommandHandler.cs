using Ardalis.GuardClauses;
using SagaPattern.Commons;
using OrdersService.Data;

namespace OrdersService.Commands;

public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly ILogger<DeleteOrderCommandHandler> _logger;

    public DeleteOrderCommandHandler(IOrdersRepository ordersRepository, ILogger<DeleteOrderCommandHandler> logger)
    {
        _ordersRepository = ordersRepository;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteOrderCommand command)
    {
        Guard.Against.Null(command);

        await _ordersRepository.DeleteOrderAsync(command.OrderId);
        _logger.LogInformation($"deleted order with id: {command.OrderId} from db");
    }
}
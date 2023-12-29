using SagaPattern.Commons;

namespace OrdersService.Commands;

public class DeleteOrderCommand : ICommand
{
    public DeleteOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
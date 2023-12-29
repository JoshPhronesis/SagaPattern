using SagaPattern.Commons;
using OrdersService.Entities;

namespace OrdersService.Commands;

public class CreateOrderCommand : ICommand
{
    public Order Order { get; }

    public CreateOrderCommand(Order order)
    {
        Order = order;
    }
}
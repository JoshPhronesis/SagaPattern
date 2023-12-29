using OrdersService.Entities;

namespace OrdersService.Data;

public interface IOrdersRepository
{
    Task CreateOrderAsync(Order order);
    Task DeleteOrderAsync(Guid orderId);
}
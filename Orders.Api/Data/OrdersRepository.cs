using Ardalis.GuardClauses;
using OrdersService.Entities;

namespace OrdersService.Data;

public class OrdersRepository : IOrdersRepository
{
    private readonly AppDbContext _dbContext;

    public OrdersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CreateOrderAsync(Order order)
    {
        Guard.Against.Null(order);

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(Guid orderId)
    {
        Guard.Against.Default(orderId);

        var order = _dbContext.Orders.FirstOrDefault(order => order.Id == orderId);
        if (order is {})
        {
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
        }
    }
}
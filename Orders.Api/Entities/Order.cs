namespace OrdersService.Entities;

public class Order
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string CustomerEmail { get; set; }
}
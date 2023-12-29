namespace OrdersService.Dtos;

public class OrderForCreateDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string CustomerEmail { get; set; }
}
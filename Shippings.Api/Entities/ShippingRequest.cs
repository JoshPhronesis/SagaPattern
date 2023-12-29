using Shipping.Api.Enums;

namespace Shipping.Api.Entities;

public class ShippingRequest
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public ShippingRequestStatus ShippingRequestStatus { get; set; }
}
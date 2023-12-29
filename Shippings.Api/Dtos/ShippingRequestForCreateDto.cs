using Shipping.Api.Enums;

namespace Shipping.Api.Dtos
{
    public class ShippingRequestForCreateDto
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public ShippingRequestStatus ShippingRequestStatus { get; set; }
    }
}
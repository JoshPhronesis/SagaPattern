using Shipping.Api.Entities;

namespace Shipping.Api.Interfaces;

public interface IShippingRequestRepository
{
    Task CreateShippingRequestAsync(ShippingRequest shippingRequest);
    Task DeleteShippingRequestAsync(Guid shippingRequestId);
}
using SagaPattern.Commons;
using Shipping.Api.Entities;

namespace Shipping.Api.Commands;

public class CreateShippingRequestCommand : ICommand
{
    public ShippingRequest ShippingRequest { get; }

    public CreateShippingRequestCommand(ShippingRequest shippingRequest)
    {
        ShippingRequest = shippingRequest;
    }
}
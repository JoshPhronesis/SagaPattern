using SagaPattern.Commons;

namespace Shipping.Api.Commands;

public class DeleteShippingRequestCommand : ICommand
{
    public Guid ShippingRequestId { get; }

    public DeleteShippingRequestCommand(Guid shippingRequestId)
    {
        ShippingRequestId = shippingRequestId;
    }
}
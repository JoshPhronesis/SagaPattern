using SagaPattern.Commons;

namespace Payments.Api.Commands;

public class DeletePaymentCommand : ICommand
{
    public Guid PaymentId { get; }

    public DeletePaymentCommand(Guid paymentId)
    {
        PaymentId = paymentId;
    }
}
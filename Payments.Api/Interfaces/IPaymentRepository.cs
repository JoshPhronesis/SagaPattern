using Payments.Api.Entities;

namespace Payments.Api.Interfaces;

public interface IPaymentRepository
{
    Task CreatePaymentDetailsAsync(PaymentDetail payment);
    Task DeletePaymentDetailsAsync(Guid paymentId);
}
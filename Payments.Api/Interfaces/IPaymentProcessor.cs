using Payments.Api.Entities;

namespace Payments.Api.Interfaces;

public interface IPaymentProcessor
{
    Task<bool> ProcessPaymentAsync(PaymentDetail paymentDetail);
}
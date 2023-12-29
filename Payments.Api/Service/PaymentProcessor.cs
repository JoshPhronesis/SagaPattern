using Payments.Api.Entities;
using Payments.Api.Enums;
using Payments.Api.Interfaces;

namespace Payments.Api.Service;

public class PaymentProcessor : IPaymentProcessor
{
    public async Task<bool> ProcessPaymentAsync(PaymentDetail paymentDetail)
    {
        await Task.Run((() =>
        {
            // simulating delay in a payment charge
            Thread.Sleep(1000);
            paymentDetail.Status = PaymentStatus.Paid;
        }));

        return true;
    }
}
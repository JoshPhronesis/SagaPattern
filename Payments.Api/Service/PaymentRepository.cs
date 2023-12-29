using Ardalis.GuardClauses;
using Payments.Api.Data;
using Payments.Api.Entities;
using Payments.Api.Interfaces;

namespace Payments.Api.Service;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _dbContext;

    public PaymentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CreatePaymentDetailsAsync(PaymentDetail payment)
    {
        Guard.Against.Null(payment);

        await _dbContext.PaymentDetails.AddAsync(payment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePaymentDetailsAsync(Guid paymentId)
    {
        Guard.Against.Default(paymentId);

        var payment = await _dbContext.PaymentDetails.FindAsync(paymentId);
        if (payment is {})
        {
            _dbContext.PaymentDetails.Remove(payment);
        }
        
        await _dbContext.SaveChangesAsync();
    }
}
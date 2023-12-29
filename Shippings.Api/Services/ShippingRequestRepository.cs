using Ardalis.GuardClauses;
using Shipping.Api.Data;
using Shipping.Api.Entities;
using Shipping.Api.Interfaces;

namespace Shipping.Api.Services;

public class ShippingRequestRepository : IShippingRequestRepository
{
    private readonly AppDbContext _dbContext;

    public ShippingRequestRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CreateShippingRequestAsync(ShippingRequest shippingRequest)
    {
        Guard.Against.Null(shippingRequest);

        await _dbContext.ShippingRequests.AddAsync(shippingRequest);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteShippingRequestAsync(Guid shippingRequestId)
    {
        Guard.Against.Default(shippingRequestId);

        var shippingRequest = await _dbContext.FindAsync<ShippingRequest>(shippingRequestId);
        if (shippingRequest is {})
        {
            _dbContext.ShippingRequests.Remove(shippingRequest);
        }
        await _dbContext.SaveChangesAsync();
    }
}
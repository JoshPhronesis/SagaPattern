using Microsoft.EntityFrameworkCore;
using Shipping.Api.Entities;

namespace Shipping.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<ShippingRequest> ShippingRequests { get; set; }
}
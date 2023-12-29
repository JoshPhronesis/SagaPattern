using Microsoft.EntityFrameworkCore;
using Payments.Api.Entities;

namespace Payments.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<PaymentDetail> PaymentDetails { get; set; }
}
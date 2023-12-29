using Microsoft.EntityFrameworkCore;
using OrdersService.Entities;

namespace OrdersService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Order> Orders { get; set; }
}
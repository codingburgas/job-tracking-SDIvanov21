using JobTracking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracking.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    
    DbSet<Application> Applications { get; set; }
    DbSet<Offer> Offers { get; set; }
    DbSet<User> Users { get; set; }
}
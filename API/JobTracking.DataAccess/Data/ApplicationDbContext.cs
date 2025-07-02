using JobTracking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracking.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public ApplicationDbContext()
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=..\\JobTracking.DataAccess\\JobTrackingDB");
    }
    
    public DbSet<Application> Applications { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<User> Users { get; set; }
}
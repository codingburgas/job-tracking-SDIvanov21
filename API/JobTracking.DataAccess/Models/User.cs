using System.Text.Json.Serialization;
using JobTracking.DataAccess.Data.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracking.DataAccess.Models;

public class User : IEntity
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string LastName { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }
    
    public string Role { get; set; }

    [JsonIgnore]
    public List<Application> Applications { get; set; } = [];
}

public class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasMany(e => e.Applications)
            .WithOne(e => e.Applicant);
    }
}

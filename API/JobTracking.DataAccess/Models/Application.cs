using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JobTracking.DataAccess.Data.Base;
using JobTracking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracking.DataAccess.Models;

public class Application : IEntity
{
    [Required]
    public int Id { get; set; }
    [Required]
    public bool IsActive { get; set; }
    
    [Required]
    public DateTime CreatedOn { get; set; }
    [Required]
    public string CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    
    [JsonIgnore]
    [Required]
    public User Applicant { get; set; }
    [JsonIgnore]
    [Required]
    public Offer Offer { get; set; }
    
    [Required]
    public ApplicationStatusEnum Status { get; set; }
}

public class ApplicationTypeConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Offer)
            .WithMany(e => e.Applications);

        builder.HasOne(e => e.Applicant)
            .WithMany(e => e.Applications);
    }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JobTracking.DataAccess.Data.Base;
using JobTracking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracking.DataAccess.Models;

public class Offer : IEntity
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
    
    [Required]
    public OfferStatusEnum Status { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public string Job { get; set; }
    [Required]
    public string Company { get; set; }
    
    [JsonIgnore]
    [Required]
    public List<Application> Applications { get; set; } = [];
}

public class OfferTypeConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Applications)
            .WithOne(x => x.Offer);
    }
}
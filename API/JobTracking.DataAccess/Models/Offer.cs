using JobTracking.DataAccess.Data.Base;
using JobTracking.Domain.Enums;

namespace JobTracking.DataAccess.Models;

public class Offer : IEntity
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    
    public OfferStatusEnum Status { get; set; }
    
    public string Description { get; set; }
    
    public string Job { get; set; }
    public string Company { get; set; }
    
    public User Poster { get; set; }
    
    public List<Application> Applications { get; set; } = [];
}
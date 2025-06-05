using System.ComponentModel.DataAnnotations;
using JobTracking.DataAccess.Data.Base;
using JobTracking.Domain.Enums;

namespace JobTracking.DataAccess.Models;

public class Application : IEntity
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    
    public User Applicant { get; set; }
    public Offer Offer { get; set; }
    
    public ApplicationStatusEnum Status { get; set; }
}

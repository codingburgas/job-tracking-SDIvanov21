using JobTracking.DataAccess.Data.Base;
using JobTracking.Domain.Enums;

namespace JobTracking.DataAccess.Data.Models;

public class Application : Entity
{
    public virtual User Applicant { get; set; }
    public virtual Offer Offer { get; set; }
    
    public ApplicationStatusEnum Status { get; set; }
}
using JobTracking.DataAccess.Data.Base;
using JobTracking.Domain.Enums;

namespace JobTracking.DataAccess.Data.Models;

public class Offer : Entity
{
    public virtual Job Job { get; set; }
    
    public OfferStatusEnum Status { get; set; }
}
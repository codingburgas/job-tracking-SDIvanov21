using JobTracking.Domain.Enums;

namespace JobTracking.Domain.DTOs;

public class Offer
{
    public string Company { get; set; }
    public string Job { get; set; }
    public string Description { get; set; }
    
    public  OfferStatusEnum Status { get; set; }
}
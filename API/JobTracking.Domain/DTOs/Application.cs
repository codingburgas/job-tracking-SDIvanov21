using JobTracking.Domain.Enums;

namespace JobTracking.Domain.DTOs;

public class Application
{
    public int ApplicantId { get; set; }  // FK to User
    public int OfferId { get; set; }      // FK to Offer
    public ApplicationStatusEnum Status { get; set; }
} 

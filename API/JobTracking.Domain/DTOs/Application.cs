using JobTracking.Domain.Enums;

namespace JobTracking.Domain.DTOs;

public class Application
{
    public int Id { get; set; }
    public int OfferId { get; set; }      // FK to Offer
    public string ApplicantUsername { get; set; }
    public ApplicationStatusEnum Status { get; set; }
}

public class ApplicationStatus
{
    public ApplicationStatusEnum Status { get; set; }
}

using JobTracking.Domain.Enums;

namespace JobTracking.Domain.DTOs;

public class UpdateOfferStatusRequest
{
    public OfferStatusEnum Status { get; set; }
} 
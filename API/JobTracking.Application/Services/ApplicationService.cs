using JobTracking.DataAccess.Data;
using JobTracking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ApplicationDTO = JobTracking.Domain.DTOs.Application;
using ApplicationModel = JobTracking.DataAccess.Models.Application;
using Microsoft.Extensions.Logging;

namespace JobTracking.Application.Services;

public interface IApplicationService
{
    Task<(bool Success, string Message)> CreateApplicationAsync(int offerId, int applicantId);
    Task<ApplicationModel?> GetApplicationByIdAsync(int id);
    Task<(bool Success, string Message)> DeleteApplicationAsync(int id);
    Task<List<ApplicationModel>> GetApplicationsByUserIdAsync(int userId);
    Task<List<ApplicationDTO>> GetApplicationsByOfferIdAsync(int offerId);
    Task<(bool Success, string Message)> UpdateApplicationStatusAsync(int id, ApplicationStatusEnum status);
}

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(ApplicationDbContext context, ILogger<ApplicationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> CreateApplicationAsync(int offerId, int applicantId)
    {
        try
        {
            var user = await _context.Users.FindAsync(applicantId);
            if (user == null)
                return (false, "Applicant not found.");

            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
                return (false, "Offer not found.");
            if (offer.Status == JobTracking.Domain.Enums.OfferStatusEnum.INACTIVE)
                return (false, "Cannot apply to an inactive offer.");

            // Prevent duplicate applications
            var existing = await _context.Applications.FirstOrDefaultAsync(a => a.Applicant.Id == applicantId && a.Offer.Id == offerId);
            if (existing != null)
                return (false, "You have already applied to this offer.");

            var application = new ApplicationModel
            {
                Applicant = user,
                Offer = offer,
                Status = JobTracking.Domain.Enums.ApplicationStatusEnum.SUBMITTED,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return (true, "Application created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application");
            return (false, "An error occurred while creating the application.");
        }
    }

    public async Task<ApplicationModel?> GetApplicationByIdAsync(int id)
    {
        try
        {
            return await _context.Applications
                .Include(a => a.Applicant)
                .Include(a => a.Offer)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting application by id {id}");
            return null;
        }
    }

    public async Task<(bool Success, string Message)> DeleteApplicationAsync(int id)
    {
        try
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return (false, "Application not found.");

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return (true, "Application deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting application with id {id}");
            return (false, "An error occurred while deleting the application.");
        }
    }

    public async Task<List<ApplicationModel>> GetApplicationsByUserIdAsync(int userId)
    {
        try
        {
            return await _context.Applications
                .Include(a => a.Offer)
                .Include(a => a.Applicant)
                .Where(a => a.Applicant.Id == userId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting applications for user {userId}");
            return new List<ApplicationModel>();
        }
    }

    public async Task<List<ApplicationDTO>> GetApplicationsByOfferIdAsync(int offerId)
    {
        try
        {
            return await _context.Applications
                .Include(a => a.Offer)
                .Include(a => a.Applicant)
                .Where(a => a.Offer.Id == offerId)
                .Select(a => new ApplicationDTO
                {
                    Id = a.Id,
                    OfferId = a.Offer.Id,
                    ApplicantUsername = a.Applicant.Username,
                    Status = a.Status
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting applications for offer {offerId}");
            return new List<ApplicationDTO>();
        }
    }

    public async Task<(bool Success, string Message)> UpdateApplicationStatusAsync(int id, ApplicationStatusEnum status)
    {
        try
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return (false, "Application not found.");
            application.Status = status;
            application.UpdatedOn = DateTime.UtcNow;
            application.UpdatedBy = "Admin";
            await _context.SaveChangesAsync();
            return (true, "Status updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating application status for id {id}");
            return (false, "An error occurred while updating the status.");
        }
    }
}
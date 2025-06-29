using JobTracking.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationDTO = JobTracking.Domain.DTOs.Application;
using ApplicationModel = JobTracking.DataAccess.Models.Application;

namespace JobTracking.Application.Services;

public interface IApplicationService
{
    Task<(bool Success, string Message)> CreateApplicationAsync(ApplicationDTO application);
    Task<ApplicationModel?> GetApplicationByIdAsync(int id);
    Task<(bool Success, string Message)> DeleteApplicationAsync(int id);
}

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;

    public ApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> CreateApplicationAsync(ApplicationDTO dto)
    {
        var user = await _context.Users.FindAsync(dto.ApplicantId);
        if (user == null)
            return (false, "Applicant not found.");

        var offer = await _context.Offers.FindAsync(dto.OfferId);
        if (offer == null)
            return (false, "Offer not found.");

        var application = new ApplicationModel
        {
            Applicant = user,
            Offer = offer,
            Status = dto.Status,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System" // Or fetch the current user if you have auth context
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return (true, "Application created successfully.");
    }

    public async Task<ApplicationModel?> GetApplicationByIdAsync(int id)
    {
        return await _context.Applications
            .Include(a => a.Applicant)
            .Include(a => a.Offer)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<(bool Success, string Message)> DeleteApplicationAsync(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return (false, "Application not found.");

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();

        return (true, "Application deleted successfully.");
    }
}
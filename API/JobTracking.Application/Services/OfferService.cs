using JobTracking.DataAccess.Data;
using JobTracking.DataAccess.Models;
using Offer = JobTracking.DataAccess.Models.Offer;
using OfferDTO = JobTracking.Domain.DTOs.Offer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobTracking.Application.Services
{
    public interface IOfferService
    {
        Task<List<Offer>> GetAllOffersAsync();
        Task<Offer?> GetOfferByIdAsync(int id);
        Task<Offer> CreateOfferAsync(OfferDTO offer, string username);
        Task<Offer?> UpdateOfferAsync(int id, OfferDTO updatedOffer, string username);
        Task<bool> DeleteOfferAsync(int id);
    }

    public class OfferService : IOfferService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OfferService> _logger;

        public OfferService(ApplicationDbContext context, ILogger<OfferService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Offer>> GetAllOffersAsync()
        {
            try
            {
                return await _context.Set<Offer>()
                    .Include(o => o.Applications)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all offers");
                return new List<Offer>();
            }
        }

        public async Task<Offer?> GetOfferByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Offer>()
                    .Include(o => o.Applications)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting offer by id {id}");
                return null;
            }
        }

        public async Task<Offer> CreateOfferAsync(OfferDTO offer, string username)
        {
            try
            {
                var newOffer = new Offer
                {
                    CreatedOn = DateTime.Now,
                    CreatedBy = username,
                    IsActive = true,
                    Company = offer.Company,
                    Description = offer.Description,
                    Job = offer.Job,
                    Status = offer.Status
                };
                _context.Set<Offer>().Add(newOffer);
                await _context.SaveChangesAsync();
                return newOffer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating offer");
                return null;
            }
        }

        public async Task<Offer?> UpdateOfferAsync(int id, OfferDTO updatedOffer, string username)
        {
            try
            {
                var existing = await _context.Set<Offer>().FindAsync(id);
                if (existing == null) return null;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = username;
                existing.Status = updatedOffer.Status;
                existing.Description = updatedOffer.Description;
                existing.Job = updatedOffer.Job;
                existing.Company = updatedOffer.Company;
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating offer with id {id}");
                return null;
            }
        }

        public async Task<bool> DeleteOfferAsync(int id)
        {
            try
            {
                var offer = await _context.Set<Offer>().FindAsync(id);
                if (offer == null) return false;
                _context.Set<Offer>().Remove(offer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting offer with id {id}");
                return false;
            }
        }
    }
}

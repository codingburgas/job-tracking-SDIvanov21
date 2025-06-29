using JobTracking.DataAccess.Data;
using JobTracking.DataAccess.Models;
using Offer = JobTracking.DataAccess.Models.Offer;
using OfferDTO = JobTracking.Domain.DTOs.Offer;
using Microsoft.EntityFrameworkCore;

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

        public OfferService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Offer>> GetAllOffersAsync()
        {
            return await _context.Set<Offer>()
                .Include(o => o.Applications)
                .ToListAsync();
        }

        public async Task<Offer?> GetOfferByIdAsync(int id)
        {
            return await _context.Set<Offer>()
                .Include(o => o.Applications)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Offer> CreateOfferAsync(OfferDTO offer, string username)
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

        public async Task<Offer?> UpdateOfferAsync(int id, OfferDTO updatedOffer, string username)
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

        public async Task<bool> DeleteOfferAsync(int id)
        {
            var offer = await _context.Set<Offer>().FindAsync(id);
            if (offer == null) return false;

            _context.Set<Offer>().Remove(offer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

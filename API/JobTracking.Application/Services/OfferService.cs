using JobTracking.DataAccess.Data;
using JobTracking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracking.Application.Services
{
    public interface IOfferService
    {
        Task<List<Offer>> GetAllOffersAsync();
        Task<Offer?> GetOfferByIdAsync(int id);
        Task<Offer> CreateOfferAsync(Offer offer);
        Task<Offer?> UpdateOfferAsync(int id, Offer updatedOffer);
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

        public async Task<Offer> CreateOfferAsync(Offer offer)
        {
            offer.CreatedOn = DateTime.UtcNow;
            _context.Set<Offer>().Add(offer);
            await _context.SaveChangesAsync();
            return offer;
        }

        public async Task<Offer?> UpdateOfferAsync(int id, Offer updatedOffer)
        {
            var existing = await _context.Set<Offer>().FindAsync(id);
            if (existing == null) return null;

            existing.UpdatedOn = DateTime.UtcNow;
            existing.UpdatedBy = updatedOffer.UpdatedBy;
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

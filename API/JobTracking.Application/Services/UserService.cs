using JobTracking.DataAccess.Data;
using JobTracking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using UserDTO = JobTracking.Domain.DTOs.User;

namespace JobTracking.Application.Services;

public interface IUserService
{
    public Task<User?> GetUsersByIdAsync(int id);
    
    public Task<User?> GetUsersByNameAsync(string name);
    public Task<List<User>?> GetAllUsersAsync();
    
    public Task<User?> UpdateUserAsync(UserDTO user);
    public Task<int> DeleteUserAsync(int id);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetUsersByIdAsync(int id)
    {
        return await _context.Set<User>()
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<User?> GetUsersByNameAsync(string name)
    {
        return await _context.Set<User>()
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.Username == name);
    }

    public async Task<List<User>?> GetAllUsersAsync()
    {
        return await _context.Set<User>()
            .Include(u => u.Applications)
            .ToListAsync();
    }

    public async Task<int> DeleteUserAsync(int id)
    {
        var offer = await _context.Set<Offer>().FindAsync(id);
        if (offer == null) return 0;

        _context.Set<Offer>().Remove(offer);
        await _context.SaveChangesAsync();
        return id;
 
    }

    public async Task<User?> UpdateUserAsync(UserDTO user)
    {
        var existing = await _context.Set<User>().FindAsync(user.Id);
        if (existing == null) return null;
        
        existing.FirstName = user.FirstName;
        existing.Surname = user.Surname;
        existing.LastName = user.LastName;
        existing.Username = user.Username;
        existing.Password = user.Password;
        
        await _context.SaveChangesAsync();
        return existing;
    }
}
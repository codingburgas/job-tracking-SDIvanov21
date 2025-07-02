using JobTracking.DataAccess.Data;
using JobTracking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using UserDTO = JobTracking.Domain.DTOs.User;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<User?> GetUsersByIdAsync(int id)
    {
        try
        {
            return await _context.Set<User>()
                .Include(u => u.Applications)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user by id {id}");
            return null;
        }
    }
    
    public async Task<User?> GetUsersByNameAsync(string name)
    {
        try
        {
            return await _context.Set<User>()
                .Include(u => u.Applications)
                .FirstOrDefaultAsync(u => u.Username == name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user by name {name}");
            return null;
        }
    }

    public async Task<List<User>?> GetAllUsersAsync()
    {
        try
        {
            return await _context.Set<User>()
                .Include(u => u.Applications)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return null;
        }
    }

    public async Task<int> DeleteUserAsync(int id)
    {
        try
        {
            var offer = await _context.Set<Offer>().FindAsync(id);
            if (offer == null) return 0;
            _context.Set<Offer>().Remove(offer);
            await _context.SaveChangesAsync();
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user with id {id}");
            return 0;
        }
    }

    public async Task<User?> UpdateUserAsync(UserDTO user)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user with id {user.Id}");
            return null;
        }
    }
}
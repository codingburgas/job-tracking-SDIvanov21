using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobTracking.DataAccess.Data;
using JobTracking.DataAccess.Models;
using DTOs = JobTracking.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;
using Microsoft.Extensions.Logging;

namespace JobTracking.Application.Services;

public interface IAuthService
{
    string GenerateToken(int userId, string username, string role);
    public Task<(bool Success, string Message)> RegisterUserAsync(DTOs.RegisterRequest userDto, bool admin = false);
    public Task<(bool Success, string Message, User? User)> LoginUserAsync(JobTracking.Domain.DTOs.LoginRequest loginDto);
}

public class AuthService : IAuthService
{
    private readonly DTOs.JwtSettings _settings;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext dbContext, IOptions<DTOs.JwtSettings> options, IPasswordHasher<User> passwordHasher, ILogger<AuthService> logger)
    {
        _settings = options.Value;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public string GenerateToken(int userId, string username, string role)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity([
                    new Claim("id", userId.ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token");
            throw;
        }
    }
    
    public async Task<(bool Success, string Message, User? User)> LoginUserAsync(DTOs.LoginRequest loginDto)
    {
        try
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password) != PasswordVerificationResult.Success)
                return (false, "Invalid username or password.", null);
            if (!user.IsActive)
                return (false, "User account is inactive.", null);
            return (true, "",  user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return (false, "An error occurred during login.", null);
        }
    } 
    
    public async Task<(bool Success, string Message)> RegisterUserAsync(DTOs.RegisterRequest userDto, bool admin = false)
    {
        try
        {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == userDto.Username);
            if (existingUser != null)
                return (false, "Username already exists.");
            var newUser = new User
            {
                FirstName = userDto.FirstName,
                Surname = userDto.Surname,
                LastName = userDto.LastName,
                Username = userDto.Username,
                Role = admin ? "Admin" : "User",
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };
            newUser.Password = _passwordHasher.HashPassword(newUser, userDto.Password);
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
            return (true, "User registered successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return (false, "An error occurred during registration.");
        }
    } 
}
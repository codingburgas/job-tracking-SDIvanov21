using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobTracking.DataAccess.Data;
using JobTracking.DataAccess.Models;
using JobTracking.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;

namespace JobTracking.Application.Services;

public interface IAuthService
{
    string GenerateToken(string username);
    public Task<(bool Success, string Message)> RegisterUserAsync(RegisterRequest userDto);
    public Task<(bool Success, string Message)> LoginUserAsync(JobTracking.Domain.DTOs.LoginRequest loginDto);
}

public class AuthService : IAuthService
{
    private readonly JwtSettings _settings;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(ApplicationDbContext dbContext, IOptions<JwtSettings> options, IPasswordHasher<User> passwordHasher)
    {
        _settings = options.Value;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public string GenerateToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, username)
                // Add other claims here if needed
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
    
    public async Task<(bool Success, string Message)> LoginUserAsync(JobTracking.Domain.DTOs.LoginRequest loginDto)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password) != PasswordVerificationResult.Success)
            return (false, "Invalid username or password.");

        if (!user.IsActive)
            return (false, "User account is inactive.");

        return (true, "");
    } 
    
    public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterRequest userDto)
    {
        // Check if user already exists
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
            Role = userDto.Role,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };
        
        newUser.Password = _passwordHasher.HashPassword(newUser, userDto.Password);

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return (true, "User registered successfully.");
    } 
}
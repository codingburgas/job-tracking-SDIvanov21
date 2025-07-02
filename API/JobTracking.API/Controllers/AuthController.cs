using JobTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = JobTracking.Domain.DTOs.LoginRequest;
using RegisterRequest = JobTracking.Domain.DTOs.RegisterRequest;
using Microsoft.Extensions.Logging;

namespace JobTracking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var result = await _authService.LoginUserAsync(loginDto);
            if (!result.Success)
                return Unauthorized(result.Message);
            var token = _authService.GenerateToken(result.User!.Id, result.User!.Username, result.User!.Role);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw;
        }
    } 
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var result = await _authService.RegisterUserAsync(userDto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            throw;
        }
    } 
    
    //[Authorize( Roles = "Admin" )]
    [HttpPost("admin/register")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var result = await _authService.RegisterUserAsync(userDto, true);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin registration");
            throw;
        }
    }
}
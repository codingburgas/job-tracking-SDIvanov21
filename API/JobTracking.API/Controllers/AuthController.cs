using JobTracking.Application.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = JobTracking.Domain.DTOs.LoginRequest;
using RegisterRequest = JobTracking.Domain.DTOs.RegisterRequest;

namespace JobTracking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
    {
        var result = await _authService.LoginUserAsync(loginDto);

        if (!result.Success)
            return Unauthorized(result.Message);

        var token = _authService.GenerateToken(loginDto.Username);
        return Ok(new { token }); // or return token later
    } 
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest userDto)
    {
        var result = await _authService.RegisterUserAsync(userDto);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    } 
}
using JobTracking.Application.Services;
using JobTracking.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserDTO = JobTracking.Domain.DTOs.User;
using Microsoft.Extensions.Logging;

namespace JobTracking.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UserDTO user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var newUser = await _userService.UpdateUserAsync(user);
            return Ok(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deletedId = await _userService.DeleteUserAsync(id);
            return Ok(deletedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user with id {id}");
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var user = await _userService.GetUsersByIdAsync(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user with id {id}");
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }
}
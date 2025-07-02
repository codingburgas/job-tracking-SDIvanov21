using JobTracking.Application.Services;
using JobTracking.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserDTO = JobTracking.Domain.DTOs.User;

namespace JobTracking.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UserDTO user)
    {
        var newUser = await _userService.UpdateUserAsync(user);

        return Ok(newUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deletedId = await _userService.DeleteUserAsync(id);

        return Ok(deletedId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _userService.GetUsersByIdAsync(id);
        
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        
        return Ok(users);
    }
}
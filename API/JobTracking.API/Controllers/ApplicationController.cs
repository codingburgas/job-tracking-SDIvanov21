using JobTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationDTO = JobTracking.Domain.DTOs.Application;
using Microsoft.Extensions.Logging;
using System.Linq;
using JobTracking.Domain.Enums;

namespace JobTracking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly IUserService _userService;
    private readonly ILogger<ApplicationController> _logger;

    public ApplicationController(IApplicationService applicationService, IUserService userService, ILogger<ApplicationController> logger)
    {
        _applicationService = applicationService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApplicationDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            // Extract applicantId from JWT claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type.EndsWith("/nameidentifier"));
            if (userIdClaim == null)
                return Unauthorized();
            var applicantId = int.Parse(userIdClaim.Value);
            // Build new DTO with only offerId, pass applicantId and status to service
            var result = await _applicationService.CreateApplicationAsync(dto.OfferId, applicantId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during application creation");
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
                return NotFound("Application not found.");
            return Ok(application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting application with id {id}");
            throw;
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _applicationService.DeleteApplicationAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting application with id {id}");
            throw;
        }
    }

    [HttpGet("for-user/{userId}")]
    public async Task<IActionResult> GetForUser(int userId)
    {
        try
        {
            var applications = await _applicationService.GetApplicationsByUserIdAsync(userId);
            var offerIds = applications.Select(a => a.Offer.Id).ToList();
            return Ok(offerIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting applications for user {userId}");
            throw;
        }
    }

    [HttpGet("for-offer/{offerId}")]
    public async Task<IActionResult> GetForOffer(int offerId)
    {
        try
        {
            var applications = await _applicationService.GetApplicationsByOfferIdAsync(offerId);
            return Ok(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting applications for offer {offerId}");
            throw;
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] JobTracking.Domain.DTOs.ApplicationStatus status)
    {
        try
        {
            var result = await _applicationService.UpdateApplicationStatusAsync(id, status.Status);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating application status for id {id}");
            throw;
        }
    }
}
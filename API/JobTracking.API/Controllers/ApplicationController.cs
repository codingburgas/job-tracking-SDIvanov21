using JobTracking.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationDTO = JobTracking.Domain.DTOs.Application;

namespace JobTracking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApplicationDTO dto)
    {
        var result = await _applicationService.CreateApplicationAsync(dto);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        if (application == null)
            return NotFound("Application not found.");

        return Ok(application);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _applicationService.DeleteApplicationAsync(id);
        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }
}
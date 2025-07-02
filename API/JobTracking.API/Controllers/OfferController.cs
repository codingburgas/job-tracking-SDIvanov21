using Offer = JobTracking.DataAccess.Models.Offer;
using OfferDTO = JobTracking.Domain.DTOs.Offer;
using JobTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JobTracking.Domain.DTOs;

namespace JobTracking.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;
        private readonly ILogger<OfferController> _logger;

        public OfferController(IOfferService offerService, ILogger<OfferController> logger)
        {
            _offerService = offerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var offers = await _offerService.GetAllOffersAsync();
                return Ok(offers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all offers");
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var offer = await _offerService.GetOfferByIdAsync(id);
                return offer == null ? NotFound() : Ok(offer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting offer with id {id}");
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] OfferDTO offer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var created = await _offerService.CreateOfferAsync(offer, User.Identity?.Name!);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating offer");
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] OfferDTO offer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var updated = await _offerService.UpdateOfferAsync(id, offer, User.Identity?.Name!);
                return updated == null ? NotFound() : Ok(User.Identity?.Name!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating offer with id {id}");
                throw;
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _offerService.DeleteOfferAsync(id);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting offer with id {id}");
                throw;
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOfferStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _offerService.UpdateOfferStatusAsync(id, request.Status);
                if (!result.Success)
                    return BadRequest(result.Message);
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, "An error occurred while updating the offer status.");
            }
        }
    }
}
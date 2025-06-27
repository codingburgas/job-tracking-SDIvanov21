using JobTracking.DataAccess.Models;
using JobTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracking.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var offers = await _offerService.GetAllOffersAsync();
            return Ok(offers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var offer = await _offerService.GetOfferByIdAsync(id);
            return offer == null ? NotFound() : Ok(offer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Offer offer)
        {
            var created = await _offerService.CreateOfferAsync(offer);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Offer offer)
        {
            var updated = await _offerService.UpdateOfferAsync(id, offer);
            return updated == null ? NotFound() : Ok(User.Identity?.Name!);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _offerService.DeleteOfferAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.Dtos;
using ParkingService.Services;

namespace ParkingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParkingLotController : ControllerBase
    {
        private readonly IParkingLotService _service;

        public ParkingLotController(IParkingLotService service)
        {
            _service = service;
        }

        // View all parking lots
        [HttpGet]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var lots = await _service.GetAllAsync();

            return Ok(lots);
        }

        // View parking lot by id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var lot = await _service.GetByIdAsync(id);

            if (lot == null)
                return NotFound("Parking lot not found.");

            return Ok(lot);
        }

        // Admin creates parking lot
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] ParkingLotDto dto)
        {
            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.LotId },
                result);
        }

        // Admin updates parking lot
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ParkingLotDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (!result)
                return NotFound("Parking lot not found.");

            return Ok("Parking lot updated successfully.");
        }

        // Admin deletes parking lot
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Parking lot not found.");

            return Ok("Parking lot deleted successfully.");
        }
    }
}
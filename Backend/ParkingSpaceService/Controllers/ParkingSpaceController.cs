using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.Services;

namespace ParkingSpaceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParkingSpaceController : ControllerBase
    {
        private readonly IParkingSpaceService _service;

        public ParkingSpaceController(
            IParkingSpaceService service)
        {
            _service = service;
        }

        [HttpPost("create-spaces")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSpaces(
            int lotId,
            int totalSlots,
            int numberOfFloors)
        {
            await _service.CreateSpacesAsync(
                lotId,
                totalSlots,
                numberOfFloors);

            return Ok("Parking spaces created successfully.");
        }

        [HttpGet("lot/{lotId}")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetSpacesByLotId(
            int lotId)
        {
            var result =
                await _service.GetSpacesByLotIdAsync(lotId);

            return Ok(result);
        }

        [HttpGet("lot/{lotId}/available")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetAvailableSpaces(
            int lotId,
            string vehicleType)
        {
            var result =
                await _service.GetAvailableSpacesAsync(
                    lotId,
                    vehicleType);

            return Ok(result);
        }

        [HttpGet("{spaceId}")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetSpaceById(
            int spaceId)
        {
            var result =
                await _service.GetSpaceByIdAsync(spaceId);

            if (result == null)
                return NotFound("Parking space not found.");

            return Ok(result);
        }

        [HttpPatch("{spaceId}/status")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> UpdateSpaceStatus(
            int spaceId,
            [FromBody] string status)
        {
            var result =
                await _service.UpdateSpaceStatusAsync(
                    spaceId,
                    status);

            if (!result)
                return BadRequest(
                    "Invalid status update.");

            return Ok("Status updated successfully.");
        }
    }
}
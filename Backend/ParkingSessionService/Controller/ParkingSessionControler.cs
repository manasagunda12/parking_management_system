using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.Dtos;
using ParkingSessionService.Services;

namespace ParkingSessionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParkingSessionController : ControllerBase
    {
        private readonly IParkingSessionService _service;

        public ParkingSessionController(
            IParkingSessionService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> GetAllSessions()
        {
            return Ok(
                await _service.GetAllSessionsAsync());
        }

        [HttpGet("{sessionId}")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetSessionById(
            int sessionId)
        {
            var session =
                await _service.GetSessionByIdAsync(sessionId);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        [HttpPost("book-slot")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BookSlot(
            [FromBody] BookSlotRequest request)
        {
            if (request == null || request.VehicleId <= 0 || request.SpaceId <= 0)
                return BadRequest("VehicleId and SpaceId are required.");

            try
            {
                var result =
                    await _service.BookSlotAsync(
                        request.VehicleId,
                        request.SpaceId);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("occupy-slot/{sessionId}")]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> OccupySlot(
            int sessionId)
        {
            var result =
                await _service.OccupySlotAsync(sessionId);

            if (!result)
                return BadRequest();

            return Ok("Slot occupied successfully.");
        }

        [HttpPut("exit-vehicle/{sessionId}")]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> ExitVehicle(
            int sessionId)
        {
            var result =
                await _service.ExitVehicleAsync(sessionId);

            if (result == null)
                return NotFound();

            return Ok(new
            {
                result.SessionId,
                result.VehicleNumber,
                result.SpaceName,
                result.EntryTime,
                result.ExitTime,
                result.Duration,
                result.Amount,
                result.Status,
                Message =
                    $"You have parked your vehicle for {result.Duration} hour(s). Amount to pay ₹{result.Amount}."
            });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.Dtos;
using ReservationService.Services;
using System.Security.Claims;

namespace ReservationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        // GET all reservations — Admin and Operator only
        [HttpGet]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _service.GetAllAsync();
            return Ok(reservations);
        }

        // GET reservations of logged in user
        [HttpGet("my")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reservations = await _service.GetByUserAsync(userId);
            return Ok(reservations);
        }

        // GET single reservation by id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _service.GetByIdAsync(id);
            if (reservation == null) return NotFound("Reservation not found.");
            return Ok(reservation);
        }

        // POST create reservation — Customer
        [HttpPost]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var created = await _service.CreateAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ReservationId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH cancel reservation
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,Operator,Customer")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await _service.CancelAsync(id, userId);
                if (!result) return NotFound("Reservation not found.");
                return Ok("Reservation cancelled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST check-in reserved customer — Operator
        [HttpPost("{id}/checkin")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> CheckIn(int id)
        {
            try
            {
                var sessionId = await _service.CheckInAsync(id);
                return Ok(new { sessionId, message = $"Check-in successful. Session ID: {sessionId}" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

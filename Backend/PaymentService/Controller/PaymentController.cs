using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Services;
using System.Security.Claims;

namespace ParkingManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(
            IPaymentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(
                await _service.GetAllAsync());
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPayments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _service.GetByUserIdAsync(userId));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment =
                await _service.GetByIdAsync(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(
            PaymentDto dto)
        {
            return Ok(
                await _service.CreateAsync(dto));
        }

        [HttpPut("{paymentId}/pay")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Pay(
            int paymentId,
            [FromQuery] string paymentMethod)
        {
            var result =
                await _service.MakePaymentAsync(
                    paymentId,
                    paymentMethod);

            if (!result)
                return BadRequest("Payment not found or already completed.");

            return Ok(new { message = "Payment completed successfully." });
        }
    }
}
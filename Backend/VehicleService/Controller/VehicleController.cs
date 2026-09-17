using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.Dtos;
using VehicleService.Services;

namespace VehicleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // Admin, Operator, Customer can view vehicles
        [Authorize(Roles = "Admin,Operator,Customer")]
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        // Admin, Operator, Customer can view a vehicle
        [Authorize(Roles = "Admin,Operator,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }

        // Customer and Admin can register a vehicle
        [Authorize(Roles = "Admin,Customer")]
        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleDto vehicleDto)
        {
            var vehicle = await _vehicleService.AddVehicleAsync(vehicleDto);

            return CreatedAtAction(
                nameof(GetVehicleById),
                new { id = vehicle.VehicleId },
                vehicle);
        }

        // Customer and Admin can update vehicle details
        [Authorize(Roles = "Admin,Customer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(
            int id,
            [FromBody] UpdateVehicleDto vehicleDto)
        {
            var result = await _vehicleService.UpdateVehicleAsync(id, vehicleDto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        // Only Admin can delete a vehicle
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
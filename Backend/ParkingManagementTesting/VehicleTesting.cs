using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagementSystem.Dtos;
using VehicleService.Controllers;
using VehicleService.Services;
using Xunit;

namespace ParkingManagementTesting
{
    public class VehicleTesting
    {
        private readonly Mock<IVehicleService> _mockService;
        private readonly VehiclesController _controller;

        public VehicleTesting()
        {
            _mockService = new Mock<IVehicleService>();
            _controller = new VehiclesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllVehicles_ReturnsOkWithList()
        {
            _mockService.Setup(s => s.GetAllVehiclesAsync())
                .ReturnsAsync(new List<VehicleDto>
                {
                    new VehicleDto { VehicleId = 1, VehicleNumber = "TN01AB1234", VehicleType = "Car", UserId = 1 }
                });

            var result = await _controller.GetAllVehicles();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<VehicleDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetVehicleById_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.GetVehicleByIdAsync(1))
                .ReturnsAsync(new VehicleDto { VehicleId = 1, VehicleNumber = "TN01AB1234", VehicleType = "Car", UserId = 1 });

            var result = await _controller.GetVehicleById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetVehicleById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetVehicleByIdAsync(99)).ReturnsAsync((VehicleDto?)null);

            var result = await _controller.GetVehicleById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddVehicle_ValidDto_ReturnsCreated()
        {
            var dto = new VehicleDto { VehicleNumber = "TN02CD5678", VehicleType = "Bike", UserId = 2 };
            _mockService.Setup(s => s.AddVehicleAsync(dto))
                .ReturnsAsync(new VehicleDto { VehicleId = 2, VehicleNumber = "TN02CD5678", VehicleType = "Bike", UserId = 2 });

            var result = await _controller.AddVehicle(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task UpdateVehicle_ExistingId_ReturnsNoContent()
        {
            var dto = new UpdateVehicleDto { VehicleNumber = "TN03EF9012", VehicleType = "Car" };
            _mockService.Setup(s => s.UpdateVehicleAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.UpdateVehicle(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateVehicle_NotFound_ReturnsNotFound()
        {
            var dto = new UpdateVehicleDto { VehicleNumber = "XX00XX0000", VehicleType = "Car" };
            _mockService.Setup(s => s.UpdateVehicleAsync(99, dto)).ReturnsAsync(false);

            var result = await _controller.UpdateVehicle(99, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteVehicle_ExistingId_ReturnsNoContent()
        {
            _mockService.Setup(s => s.DeleteVehicleAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteVehicle(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteVehicle_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteVehicleAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeleteVehicle(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}

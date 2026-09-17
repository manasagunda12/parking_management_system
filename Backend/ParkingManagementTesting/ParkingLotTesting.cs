using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagementSystem.Dtos;
using ParkingService.Controllers;
using ParkingService.Services;
using Xunit;

namespace ParkingManagementTesting
{
    public class ParkingLotTesting
    {
        private readonly Mock<IParkingLotService> _mockService;
        private readonly ParkingLotController _controller;

        public ParkingLotTesting()
        {
            _mockService = new Mock<IParkingLotService>();
            _controller = new ParkingLotController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<ParkingLotDto>
                {
                    new ParkingLotDto { LotId = 1, Name = "Lot A", Location = "Downtown", TotalSlots = 50 }
                });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<ParkingLotDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new ParkingLotDto { LotId = 1, Name = "Lot A", Location = "Downtown", TotalSlots = 50 });

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ParkingLotDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsCreated()
        {
            var dto = new ParkingLotDto { Name = "Lot B", Location = "Uptown", TotalSlots = 100, NumberOfFloors = 2 };
            _mockService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(new ParkingLotDto { LotId = 2, Name = "Lot B", Location = "Uptown", TotalSlots = 100 });

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ExistingId_ReturnsOk()
        {
            var dto = new ParkingLotDto { Name = "Lot A Updated", Location = "Downtown", TotalSlots = 60 };
            _mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.Update(1, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNotFound()
        {
            var dto = new ParkingLotDto { Name = "Ghost", Location = "Nowhere", TotalSlots = 10 };
            _mockService.Setup(s => s.UpdateAsync(99, dto)).ReturnsAsync(false);

            var result = await _controller.Update(99, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

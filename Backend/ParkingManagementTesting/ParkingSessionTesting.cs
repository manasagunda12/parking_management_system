using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagementSystem.Dtos;
using ParkingSessionService.Controllers;
using ParkingSessionService.Services;
using Xunit;

namespace ParkingManagementTesting
{
    public class ParkingSessionTesting
    {
        private readonly Mock<IParkingSessionService> _mockService;
        private readonly ParkingSessionController _controller;

        public ParkingSessionTesting()
        {
            _mockService = new Mock<IParkingSessionService>();
            _controller = new ParkingSessionController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllSessions_ReturnsOkWithList()
        {
            _mockService.Setup(s => s.GetAllSessionsAsync())
                .ReturnsAsync(new List<ParkingSessionDto>
                {
                    new ParkingSessionDto { SessionId = 1, VehicleId = 1, SpaceId = 1, Status = "Booked" }
                });

            var result = await _controller.GetAllSessions();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<ParkingSessionDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetSessionById_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.GetSessionByIdAsync(1))
                .ReturnsAsync(new ParkingSessionDto { SessionId = 1, VehicleId = 1, SpaceId = 1, Status = "Booked" });

            var result = await _controller.GetSessionById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetSessionById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetSessionByIdAsync(99)).ReturnsAsync((ParkingSessionDto?)null);

            var result = await _controller.GetSessionById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task BookSlot_ValidRequest_ReturnsOk()
        {
            var request = new BookSlotRequest { VehicleId = 1, SpaceId = 1 };
            _mockService.Setup(s => s.BookSlotAsync(1, 1))
                .ReturnsAsync(new ParkingSessionDto { SessionId = 1, VehicleId = 1, SpaceId = 1, Status = "Booked" });

            var result = await _controller.BookSlot(request);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task BookSlot_InvalidRequest_ReturnsBadRequest()
        {
            var result = await _controller.BookSlot(new BookSlotRequest { VehicleId = 0, SpaceId = 0 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task OccupySlot_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.OccupySlotAsync(1)).ReturnsAsync(true);

            var result = await _controller.OccupySlot(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task OccupySlot_Invalid_ReturnsBadRequest()
        {
            _mockService.Setup(s => s.OccupySlotAsync(99)).ReturnsAsync(false);

            var result = await _controller.OccupySlot(99);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ExitVehicle_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.ExitVehicleAsync(1))
                .ReturnsAsync(new ParkingSessionDto
                {
                    SessionId = 1, VehicleNumber = "TN01AB1234", SpaceName = "A-01",
                    EntryTime = "2025-01-01 10:00", ExitTime = "2025-01-01 12:00",
                    Duration = 2, Amount = 40, Status = "Completed"
                });

            var result = await _controller.ExitVehicle(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ExitVehicle_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.ExitVehicleAsync(99)).ReturnsAsync((ParkingSessionDto?)null);

            var result = await _controller.ExitVehicle(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}

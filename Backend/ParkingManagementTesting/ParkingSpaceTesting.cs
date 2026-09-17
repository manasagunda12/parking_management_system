using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Services;
using ParkingSpaceService.Controllers;
using Xunit;

namespace ParkingManagementTesting
{
    public class ParkingSpaceTesting
    {
        private readonly Mock<IParkingSpaceService> _mockService;
        private readonly ParkingSpaceController _controller;

        public ParkingSpaceTesting()
        {
            _mockService = new Mock<IParkingSpaceService>();
            _controller = new ParkingSpaceController(_mockService.Object);
        }

        [Fact]
        public async Task CreateSpaces_ValidInput_ReturnsOk()
        {
            _mockService.Setup(s => s.CreateSpacesAsync(1, 20, 2)).Returns(Task.CompletedTask);

            var result = await _controller.CreateSpaces(1, 20, 2);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetSpacesByLotId_ReturnsOkWithList()
        {
            _mockService.Setup(s => s.GetSpacesByLotIdAsync(1))
                .ReturnsAsync(new List<ParkingSpaceDto>
                {
                    new ParkingSpaceDto { SpaceId = 1, SpaceName = "A-01", SpaceType = "Car", Status = "Available", LotId = 1 }
                });

            var result = await _controller.GetSpacesByLotId(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<ParkingSpaceDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetAvailableSpaces_ReturnsOk()
        {
            _mockService.Setup(s => s.GetAvailableSpacesAsync(1, "Car"))
                .ReturnsAsync(new List<ParkingSpaceDto>
                {
                    new ParkingSpaceDto { SpaceId = 2, SpaceName = "A-02", SpaceType = "Car", Status = "Available", LotId = 1 }
                });

            var result = await _controller.GetAvailableSpaces(1, "Car");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetSpaceById_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.GetSpaceByIdAsync(1))
                .ReturnsAsync(new ParkingSpaceDto { SpaceId = 1, SpaceName = "A-01", SpaceType = "Car", Status = "Available", LotId = 1 });

            var result = await _controller.GetSpaceById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetSpaceById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetSpaceByIdAsync(99)).ReturnsAsync((ParkingSpaceDto?)null);

            var result = await _controller.GetSpaceById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateSpaceStatus_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.UpdateSpaceStatusAsync(1, "Occupied")).ReturnsAsync(true);

            var result = await _controller.UpdateSpaceStatus(1, "Occupied");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateSpaceStatus_Invalid_ReturnsBadRequest()
        {
            _mockService.Setup(s => s.UpdateSpaceStatusAsync(1, "BadStatus")).ReturnsAsync(false);

            var result = await _controller.UpdateSpaceStatus(1, "BadStatus");

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

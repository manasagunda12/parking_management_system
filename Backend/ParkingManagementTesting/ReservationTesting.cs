using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagementSystem.Dtos;
using ReservationService.Controllers;
using ReservationService.Services;
using System.Security.Claims;
using Xunit;

namespace ParkingManagementTesting
{
    public class ReservationTesting
    {
        private readonly Mock<IReservationService> _mockService;
        private readonly ReservationController _controller;

        public ReservationTesting()
        {
            _mockService = new Mock<IReservationService>();
            _controller = new ReservationController(_mockService.Object);
        }

        private void SetUser(int userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<ReservationDto>
                {
                    new ReservationDto { ReservationId = 1, UserId = 1, SpaceId = 1, Status = "Active" }
                });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<ReservationDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetMyReservations_ReturnsOk()
        {
            SetUser(1);
            _mockService.Setup(s => s.GetByUserAsync(1))
                .ReturnsAsync(new List<ReservationDto>
                {
                    new ReservationDto { ReservationId = 1, UserId = 1, SpaceId = 1, Status = "Active" }
                });

            var result = await _controller.GetMyReservations();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new ReservationDto { ReservationId = 1, UserId = 1, SpaceId = 1, Status = "Active" });

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ReservationDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsCreated()
        {
            SetUser(1);
            var dto = new CreateReservationDto
            {
                VehicleId = 1, SpaceId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3)
            };
            _mockService.Setup(s => s.CreateAsync(1, dto))
                .ReturnsAsync(new ReservationDto { ReservationId = 1, UserId = 1, SpaceId = 1, Status = "Active" });

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Cancel_ExistingReservation_ReturnsOk()
        {
            SetUser(1);
            _mockService.Setup(s => s.CancelAsync(1, 1)).ReturnsAsync(true);

            var result = await _controller.Cancel(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Cancel_NotFound_ReturnsNotFound()
        {
            SetUser(1);
            _mockService.Setup(s => s.CancelAsync(99, 1)).ReturnsAsync(false);

            var result = await _controller.Cancel(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CheckIn_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.CheckInAsync(1)).ReturnsAsync(10);

            var result = await _controller.CheckIn(1);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}

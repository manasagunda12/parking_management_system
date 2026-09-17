using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagementSystem.Controllers;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Services;
using System.Security.Claims;
using Xunit;

namespace ParkingManagementTesting
{
    public class PaymentTesting
    {
        private readonly Mock<IPaymentService> _mockService;
        private readonly PaymentController _controller;

        public PaymentTesting()
        {
            _mockService = new Mock<IPaymentService>();
            _controller = new PaymentController(_mockService.Object);
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
                .ReturnsAsync(new List<PaymentDto>
                {
                    new PaymentDto { PaymentId = 1, Amount = 100, Status = "Pending", SessionId = 1 }
                });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<PaymentDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetMyPayments_ReturnsOk()
        {
            SetUser(1);
            _mockService.Setup(s => s.GetByUserIdAsync(1))
                .ReturnsAsync(new List<PaymentDto>
                {
                    new PaymentDto { PaymentId = 1, Amount = 100, Status = "Pending", SessionId = 1 }
                });

            var result = await _controller.GetMyPayments();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new PaymentDto { PaymentId = 1, Amount = 100, Status = "Pending", SessionId = 1 });

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((PaymentDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsOk()
        {
            var dto = new PaymentDto { Amount = 200, PaymentMethod = "Cash", Status = "Pending", SessionId = 2 };
            _mockService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(new PaymentDto { PaymentId = 2, Amount = 200, Status = "Pending", SessionId = 2 });

            var result = await _controller.Create(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Pay_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.MakePaymentAsync(1, "UPI")).ReturnsAsync(true);

            var result = await _controller.Pay(1, "UPI");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Pay_Invalid_ReturnsBadRequest()
        {
            _mockService.Setup(s => s.MakePaymentAsync(99, "UPI")).ReturnsAsync(false);

            var result = await _controller.Pay(99, "UPI");

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

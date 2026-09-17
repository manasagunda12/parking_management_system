using InvoiceService.Controllers;
using InvoiceService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingManagementSystem.Dtos;
using System.Security.Claims;
using Xunit;

namespace ParkingManagementTesting
{
    public class InvoiceTesting
    {
        private readonly Mock<IInvoiceService> _mockService;
        private readonly InvoiceController _controller;

        public InvoiceTesting()
        {
            _mockService = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_mockService.Object);
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
        public async Task GenerateInvoice_ValidPaymentId_ReturnsOk()
        {
            _mockService.Setup(s => s.GenerateInvoiceAsync(1))
                .ReturnsAsync(new InvoiceDto { InvoiceId = 1, PaymentId = 1, TotalAmount = 200, Date = DateTime.Now });

            var result = await _controller.GenerateInvoice(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var invoice = Assert.IsType<InvoiceDto>(ok.Value);
            Assert.Equal(1, invoice.InvoiceId);
        }

        [Fact]
        public async Task GetInvoiceById_ExistingId_ReturnsOk()
        {
            _mockService.Setup(s => s.GetInvoiceByIdAsync(1))
                .ReturnsAsync(new InvoiceDto { InvoiceId = 1, PaymentId = 1, TotalAmount = 200, Date = DateTime.Now });

            var result = await _controller.GetInvoiceById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetInvoiceById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetInvoiceByIdAsync(99)).ReturnsAsync((InvoiceDto?)null);

            var result = await _controller.GetInvoiceById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetMyInvoices_ReturnsOkWithList()
        {
            SetUser(1);
            _mockService.Setup(s => s.GetMyInvoicesAsync(1))
                .ReturnsAsync(new List<InvoiceDto>
                {
                    new InvoiceDto { InvoiceId = 1, PaymentId = 1, TotalAmount = 200, Date = DateTime.Now }
                });

            var result = await _controller.GetMyInvoices();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<InvoiceDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetAllInvoices_ReturnsOkWithList()
        {
            _mockService.Setup(s => s.GetAllInvoicesAsync())
                .ReturnsAsync(new List<InvoiceDto>
                {
                    new InvoiceDto { InvoiceId = 1, PaymentId = 1, TotalAmount = 200, Date = DateTime.Now },
                    new InvoiceDto { InvoiceId = 2, PaymentId = 2, TotalAmount = 150, Date = DateTime.Now }
                });

            var result = await _controller.GetAllInvoices();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<InvoiceDto>>(ok.Value);
            Assert.Equal(2, list.Count());
        }
    }
}

using AuthService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;
using Xunit;

namespace ParkingManagementTesting
{
    public class AuthTesting
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        private IConfiguration GetConfiguration()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKey1234567890!@#$%^&*");
            config.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            config.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            return config.Object;
        }

        [Fact]
        public async Task Register_NewUser_ReturnsOk()
        {
            var context = GetInMemoryContext("Auth_Register");
            var controller = new AuthController(context, GetConfiguration());

            var result = await controller.Register(new RegisterRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                Password = "password123"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            var context = GetInMemoryContext("Auth_Duplicate");
            context.Users.Add(new User
            {
                Name = "Existing",
                Email = "dup@example.com",
                PhoneNumber = "1234567890",
                Password = BCrypt.Net.BCrypt.HashPassword("pass"),
                Role = "Customer"
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context, GetConfiguration());

            var result = await controller.Register(new RegisterRequest
            {
                Name = "New",
                Email = "dup@example.com",
                PhoneNumber = "9876543210",
                Password = "newpass123"
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var context = GetInMemoryContext("Auth_Login");
            context.Users.Add(new User
            {
                Name = "Login User",
                Email = "login@example.com",
                PhoneNumber = "1234567890",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Customer"
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context, GetConfiguration());

            var result = await controller.Login(new LoginRequest
            {
                Email = "login@example.com",
                Password = "password123"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Login_InvalidEmail_ReturnsUnauthorized()
        {
            var context = GetInMemoryContext("Auth_InvalidEmail");
            var controller = new AuthController(context, GetConfiguration());

            var result = await controller.Login(new LoginRequest
            {
                Email = "notfound@example.com",
                Password = "password123"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            var context = GetInMemoryContext("Auth_WrongPass");
            context.Users.Add(new User
            {
                Name = "User",
                Email = "user@example.com",
                PhoneNumber = "1234567890",
                Password = BCrypt.Net.BCrypt.HashPassword("correctpass"),
                Role = "Customer"
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context, GetConfiguration());

            var result = await controller.Login(new LoginRequest
            {
                Email = "user@example.com",
                Password = "wrongpass"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk()
        {
            var context = GetInMemoryContext("Auth_GetAll");
            context.Users.Add(new User
            {
                Name = "Admin",
                Email = "admin@example.com",
                PhoneNumber = "1234567890",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin"
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context, GetConfiguration());

            var result = await controller.GetAllUsers();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}

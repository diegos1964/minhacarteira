using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.Controllers;
using MinhaCarteira.API.DTOs.Auth;
using Moq;
using Xunit;

namespace MinhaCarteira.Tests.Controllers
{
  public class AuthControllerTests : TestBase
  {
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
      _controller = new AuthController(AuthService.Object);

      // Setup mock behaviors
      AuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDTO>()))
          .ReturnsAsync(new AuthResponseDTO
          {
            Name = TestUser.Name,
            Email = TestUser.Email,
            Token = "test-token"
          });

      AuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDTO>()))
          .ReturnsAsync(new AuthResponseDTO
          {
            Name = TestUser.Name,
            Email = TestUser.Email,
            Token = "test-token"
          });
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOkResult()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@email.com",
        Password = "123456"
      };

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var actionResult = Assert.IsType<ActionResult<AuthResponseDTO>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var response = Assert.IsType<AuthResponseDTO>(okResult.Value);
      Assert.NotNull(response.Token);
      Assert.Equal(registerDto.Email, response.Email);
      Assert.Equal(registerDto.Name, response.Name);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@email.com",
        Password = "123456"
      };

      AuthService.Setup(x => x.RegisterAsync(registerDto))
          .ThrowsAsync(new InvalidOperationException("Email already exists"));

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var actionResult = Assert.IsType<ActionResult<AuthResponseDTO>>(result);
      Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
      // Arrange
      var loginDto = new LoginDTO
      {
        Email = "test@email.com",
        Password = "123456"
      };

      // Act
      var result = await _controller.Login(loginDto);

      // Assert
      var actionResult = Assert.IsType<ActionResult<AuthResponseDTO>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var response = Assert.IsType<AuthResponseDTO>(okResult.Value);
      Assert.NotNull(response.Token);
      Assert.Equal(loginDto.Email, response.Email);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
      // Arrange
      var loginDto = new LoginDTO
      {
        Email = "nonexistent@email.com",
        Password = "wrongpassword"
      };

      AuthService.Setup(x => x.LoginAsync(loginDto))
          .ThrowsAsync(new InvalidOperationException("Invalid credentials"));

      // Act
      var result = await _controller.Login(loginDto);

      // Assert
      var actionResult = Assert.IsType<ActionResult<AuthResponseDTO>>(result);
      Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
  }
}
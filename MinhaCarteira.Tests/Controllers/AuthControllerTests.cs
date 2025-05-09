using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MinhaCarteira.API.Controllers;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Services;
using Xunit;
using MinhaCarteira.API.DTOs.Reponses;

namespace MinhaCarteira.Tests.Controllers
{
  public class AuthControllerTests : TestBase
  {
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
      _mockAuthService = new Mock<IAuthService>();
      _controller = new AuthController(_mockAuthService.Object);

      // Setup controller user claims
      _controller.ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = TestUserClaims }
      };
    }

    [Fact]
    public async Task Register_ValidData_ReturnsOkResult()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = "Test123!",
        ConfirmPassword = "Test123!",
        CPF = "12345678900"
      };

      var authResponse = new AuthResponseDTO
      {
        Token = "test-token",
        User = new UserDTO
        {
          Id = 1,
          Name = registerDto.Name,
          Email = registerDto.Email,
          CPF = registerDto.CPF,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        }
      };

      _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
          .ReturnsAsync(authResponse);

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<AuthResponseDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(authResponse.Token, response.Data!.Token);
      Assert.Equal(authResponse.User.Id, response.Data.User.Id);
      Assert.Equal(authResponse.User.Name, response.Data.User.Name);
      Assert.Equal(authResponse.User.Email, response.Data.User.Email);
      Assert.Equal(authResponse.User.CPF, response.Data.User.CPF);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = "Test123!",
        ConfirmPassword = "Test123!",
        CPF = "12345678900"
      };

      _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
          .ThrowsAsync(new InvalidOperationException("Email already registered"));

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Null(response.Data);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
      // Arrange
      var loginDto = new LoginDTO
      {
        Email = "test@example.com",
        Password = "Test123!"
      };

      var authResponse = new AuthResponseDTO
      {
        Token = "test-token",
        User = new UserDTO
        {
          Id = 1,
          Name = "Test User",
          Email = loginDto.Email,
          CPF = "12345678900",
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        }
      };

      _mockAuthService.Setup(x => x.LoginAsync(loginDto))
          .ReturnsAsync(authResponse);

      // Act
      var result = await _controller.Login(loginDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<AuthResponseDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(authResponse.Token, response.Data!.Token);
      Assert.Equal(authResponse.User.Id, response.Data.User.Id);
      Assert.Equal(authResponse.User.Name, response.Data.User.Name);
      Assert.Equal(authResponse.User.Email, response.Data.User.Email);
      Assert.Equal(authResponse.User.CPF, response.Data.User.CPF);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
      // Arrange
      var loginDto = new LoginDTO
      {
        Email = "test@example.com",
        Password = "WrongPassword123!"
      };

      _mockAuthService.Setup(x => x.LoginAsync(loginDto))
          .ThrowsAsync(new InvalidOperationException("Invalid credentials"));

      // Act
      var result = await _controller.Login(loginDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Null(response.Data);
    }

    [Fact]
    public async Task GetCurrentUser_ValidToken_ReturnsOkResult()
    {
      // Arrange
      var userDto = new UserDTO
      {
        Id = TestUser.Id,
        Name = TestUser.Name,
        Email = TestUser.Email,
        CPF = TestUser.CPF,
        CreatedAt = TestUser.CreatedAt,
        UpdatedAt = TestUser.CreatedAt
      };

      _mockAuthService.Setup(x => x.GetUserByIdAsync(TestUser.Id))
          .ReturnsAsync(userDto);

      // Act
      var result = await _controller.GetCurrentUser();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<UserDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(TestUser.Id, response.Data!.Id);
      Assert.Equal(TestUser.Name, response.Data.Name);
      Assert.Equal(TestUser.Email, response.Data.Email);
      Assert.Equal(TestUser.CPF, response.Data.CPF);
    }
  }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using MinhaCarteira.API.Controllers;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Services;
using Xunit;
using MinhaCarteira.API.DTOs.Reponses;
using MinhaCarteira.API.Validations;

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

    private void ValidateModel(object model)
    {
      var validationContext = new ValidationContext(model, null, null);
      var validationResults = new List<ValidationResult>();
      Validator.TryValidateObject(model, validationContext, validationResults, true);

      // Adiciona erros de validação ao ModelState
      foreach (var validationResult in validationResults)
      {
        foreach (var memberName in validationResult.MemberNames)
        {
          _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage!);
        }
      }

      // Validações específicas para RegisterDTO
      if (model is RegisterDTO registerDto)
      {
        // Validação de CPF
        var cpfValidator = new CPFAttribute();
        if (!cpfValidator.IsValid(registerDto.CPF))
        {
          _controller.ModelState.AddModelError("CPF", "CPF inválido");
        }

        // Validação de senha
        if (registerDto.Password != registerDto.ConfirmPassword)
        {
          _controller.ModelState.AddModelError("ConfirmPassword", "As senhas não conferem");
        }

        // Validação de força da senha
        var passwordRegex = new RegularExpressionAttribute(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
        if (!passwordRegex.IsValid(registerDto.Password))
        {
          _controller.ModelState.AddModelError("Password", "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial");
        }
      }
    }

    #region Register Tests

    [Fact]
    public async Task Register_ValidData_ReturnsOkResult()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = "Test@123",
        ConfirmPassword = "Test@123",
        CPF = "52998224725"
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

    [Theory]
    [InlineData("", "O nome é obrigatório")]
    [InlineData("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat", "O nome deve ter no máximo 100 caracteres")]
    public async Task Register_InvalidName_ReturnsBadRequest(string name, string expectedError)
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = name,
        Email = "test@example.com",
        Password = "Test@123",
        ConfirmPassword = "Test@123",
        CPF = "52998224725"
      };

      ValidateModel(registerDto);

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Contains(expectedError, response.Message);
    }

    [Theory]
    [InlineData("", "O email é obrigatório")]
    [InlineData("invalid-email", "Email inválido")]
    public async Task Register_InvalidEmail_ReturnsBadRequest(string email, string expectedError)
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = email,
        Password = "Test@123",
        ConfirmPassword = "Test@123",
        CPF = "52998224725"
      };

      ValidateModel(registerDto);

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Contains(expectedError, response.Message);
    }

    [Theory]
    [InlineData("12345678900", "CPF inválido")] // CPF com dígitos verificadores inválidos
    [InlineData("11111111111", "CPF inválido")] // CPF com todos os dígitos iguais
    [InlineData("123456789", "CPF inválido")]   // CPF com menos de 11 dígitos
    [InlineData("123456789012", "CPF inválido")] // CPF com mais de 11 dígitos
    [InlineData("123.456.789-00", "CPF inválido")] // CPF com formatação inválida
    public async Task Register_InvalidCPF_ReturnsBadRequest(string cpf, string expectedError)
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = "Test@123",
        ConfirmPassword = "Test@123",
        CPF = cpf
      };

      ValidateModel(registerDto);

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Contains(expectedError, response.Message);
    }

    [Theory]
    [InlineData("weak", "A senha deve ter entre 8 e 100 caracteres")]
    [InlineData("weakpass", "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial")]
    [InlineData("WeakPass", "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial")]
    [InlineData("WeakPass1", "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial")]
    public async Task Register_InvalidPassword_ReturnsBadRequest(string password, string expectedError)
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = password,
        ConfirmPassword = password,
        CPF = "52998224725"
      };

      ValidateModel(registerDto);

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Contains(expectedError, response.Message);
    }

    [Fact]
    public async Task Register_PasswordMismatch_ReturnsBadRequest()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = "Test@123",
        ConfirmPassword = "Test@456",
        CPF = "52998224725"
      };

      ValidateModel(registerDto);

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Contains("As senhas não conferem", response.Message);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = "Test@123",
        ConfirmPassword = "Test@123",
        CPF = "52998224725"
      };

      _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
          .ThrowsAsync(new InvalidOperationException("Email já está em uso"));

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Contains("Email já está em uso", response.Message);
    }

    [Fact]
    public async Task Register_InternalError_ReturnsInternalServerError()
    {
      // Arrange
      var registerDto = new RegisterDTO
      {
        Name = "Test User",
        Email = "test@example.com",
        Password = "Test@123",
        ConfirmPassword = "Test@123",
        CPF = "52998224725"
      };

      _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
          .ThrowsAsync(new Exception("Erro interno"));

      // Act
      var result = await _controller.Register(registerDto);

      // Assert
      var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
      Assert.Equal(500, statusCodeResult.StatusCode);
      var response = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
      Assert.False(response.Success);
      Assert.Contains("Ocorreu um erro interno", response.Message);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
      // Arrange
      var loginDto = new LoginDTO
      {
        Email = "test@example.com",
        Password = "Test@123"
      };

      var authResponse = new AuthResponseDTO
      {
        Token = "test-token",
        User = new UserDTO
        {
          Id = 1,
          Name = "Test User",
          Email = loginDto.Email,
          CPF = "52998224725",
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
        Password = "WrongPassword@123"
      };

      _mockAuthService.Setup(x => x.LoginAsync(loginDto))
          .ThrowsAsync(new InvalidOperationException("Credenciais inválidas"));

      // Act
      var result = await _controller.Login(loginDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
      Assert.Contains("Credenciais inválidas", response.Message);
    }

    [Fact]
    public async Task Login_InternalError_ReturnsInternalServerError()
    {
      // Arrange
      var loginDto = new LoginDTO
      {
        Email = "test@example.com",
        Password = "Test@123"
      };

      _mockAuthService.Setup(x => x.LoginAsync(loginDto))
          .ThrowsAsync(new Exception("Erro interno"));

      // Act
      var result = await _controller.Login(loginDto);

      // Assert
      var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
      Assert.Equal(500, statusCodeResult.StatusCode);
      var response = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
      Assert.False(response.Success);
      Assert.Contains("Ocorreu um erro interno", response.Message);
    }

    #endregion

    #region GetCurrentUser Tests

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

    [Fact]
    public async Task GetCurrentUser_UserNotFound_ReturnsNotFound()
    {
      // Arrange
      _mockAuthService.Setup(x => x.GetUserByIdAsync(TestUser.Id))
          .ReturnsAsync((UserDTO)null!);

      // Act
      var result = await _controller.GetCurrentUser();

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
      Assert.False(response.Success);
      Assert.Contains("Usuário não encontrado", response.Message);
    }

    [Fact]
    public async Task GetCurrentUser_InternalError_ReturnsInternalServerError()
    {
      // Arrange
      _mockAuthService.Setup(x => x.GetUserByIdAsync(TestUser.Id))
          .ThrowsAsync(new Exception("Erro interno"));

      // Act
      var result = await _controller.GetCurrentUser();

      // Assert
      var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
      Assert.Equal(500, statusCodeResult.StatusCode);
      var response = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
      Assert.False(response.Success);
      Assert.Contains("Ocorreu um erro interno", response.Message);
    }

    #endregion
  }
}
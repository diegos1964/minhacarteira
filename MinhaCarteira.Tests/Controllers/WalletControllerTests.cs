using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinhaCarteira.API.Controllers;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.DTOs.Reponses;
using MinhaCarteira.API.DTOs.Wallet;
using MinhaCarteira.API.Exceptions;
using MinhaCarteira.API.Services;
using Moq;
using Xunit;

namespace MinhaCarteira.Tests.Controllers
{
  public class WalletControllerTests : TestBase
  {
    private readonly Mock<IWalletService> _mockWalletService;
    private readonly Mock<ILogger<WalletController>> _mockLogger;
    private readonly WalletController _controller;

    public WalletControllerTests()
    {
      _mockWalletService = new Mock<IWalletService>();
      _mockLogger = new Mock<ILogger<WalletController>>();
      _controller = new WalletController(_mockWalletService.Object, _mockLogger.Object);

      // Setup controller user claims
      var claims = new List<Claim>
            {
                new Claim("id", TestUser.Id.ToString()),
                new Claim(ClaimTypes.Email, TestUser.Email)
            };
      var identity = new ClaimsIdentity(claims, "TestAuth");
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _controller.ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = claimsPrincipal }
      };
    }

    [Fact]
    public async Task GetWallets_ValidRequest_ReturnsOkResult()
    {
      // Arrange
      var filter = new WalletFilterDTO { PageNumber = 1, PageSize = 10 };
      var expectedResponse = new PaginatedResultDTO<WalletDTO>
      {
        Items = new List<WalletDTO>
                {
                    new WalletDTO
                    {
                        Id = TestWallet.Id,
                        Name = TestWallet.Name,
                        Balance = TestWallet.Balance,
                        CreatedAt = TestWallet.CreatedAt,
                        UpdatedAt = TestWallet.UpdatedAt
                    }
                },
        TotalItems = 1,
        PageNumber = 1,
        PageSize = 10,
        TotalPages = 1
      };

      _mockWalletService.Setup(x => x.GetUserWalletsAsync(TestUser.Id, It.IsAny<WalletFilterDTO>()))
          .ReturnsAsync(expectedResponse);

      // Act
      var result = await _controller.GetWallets(filter);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<PaginatedResultDTO<WalletDTO>>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Single(response.Data.Items);
      Assert.Equal(TestWallet.Id, response.Data.Items.First().Id);
    }

    [Fact]
    public async Task GetWallet_ExistingWallet_ReturnsOkResult()
    {
      // Arrange
      var walletDto = new WalletDTO
      {
        Id = TestWallet.Id,
        Name = TestWallet.Name,
        Balance = TestWallet.Balance,
        CreatedAt = TestWallet.CreatedAt,
        UpdatedAt = TestWallet.UpdatedAt
      };

      _mockWalletService.Setup(x => x.GetWalletAsync(TestWallet.Id, TestUser.Id))
          .ReturnsAsync(walletDto);

      // Act
      var result = await _controller.GetWallet(TestWallet.Id);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<WalletDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(TestWallet.Id, response.Data.Id);
    }

    [Fact]
    public async Task GetWallet_NonExistingWallet_ReturnsNotFound()
    {
      // Arrange
      _mockWalletService.Setup(x => x.GetWalletAsync(It.IsAny<int>(), TestUser.Id))
          .ReturnsAsync((WalletDTO?)null);

      // Act
      var result = await _controller.GetWallet(999);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public async Task CreateWallet_ValidData_ReturnsCreatedResult()
    {
      // Arrange
      var createDto = new CreateWalletDTO
      {
        Name = "New Wallet",
        InitialBalance = 100m
      };

      var createdWallet = new WalletDTO
      {
        Id = 2,
        Name = createDto.Name,
        Balance = createDto.InitialBalance ?? 0m,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = null
      };

      _mockWalletService.Setup(x => x.CreateWalletAsync(createDto, TestUser.Id))
          .ReturnsAsync(createdWallet);

      // Act
      var result = await _controller.CreateWallet(createDto);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
      var response = Assert.IsType<ApiResponse<WalletDTO>>(createdResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(createDto.Name, response.Data.Name);
      Assert.Equal(createDto.InitialBalance ?? 0m, response.Data.Balance);
    }

    [Fact]
    public async Task UpdateWallet_ValidData_ReturnsOkResult()
    {
      // Arrange
      var updateDto = new UpdateWalletDTO { Name = "Updated Wallet" };

      _mockWalletService.Setup(x => x.UpdateWalletAsync(TestWallet.Id, updateDto, TestUser.Id))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateWallet(TestWallet.Id, updateDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
      Assert.True(response.Success);
    }

    [Fact]
    public async Task UpdateWallet_NonExistingWallet_ReturnsBadRequest()
    {
      // Arrange
      var updateDto = new UpdateWalletDTO { Name = "Updated Wallet" };

      _mockWalletService.Setup(x => x.UpdateWalletAsync(999, updateDto, TestUser.Id))
          .ThrowsAsync(new InvalidOperationException("Carteira não encontrada"));

      // Act
      var result = await _controller.UpdateWallet(999, updateDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public async Task DeleteWallet_ExistingWallet_ReturnsOkResult()
    {
      // Arrange
      _mockWalletService.Setup(x => x.DeleteWalletAsync(TestWallet.Id, TestUser.Id))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteWallet(TestWallet.Id);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
      Assert.True(response.Success);
    }

    [Fact]
    public async Task DeleteWallet_NonExistingWallet_ReturnsBadRequest()
    {
      // Arrange
      _mockWalletService.Setup(x => x.DeleteWalletAsync(999, TestUser.Id))
          .ThrowsAsync(new InvalidOperationException("Carteira não encontrada"));

      // Act
      var result = await _controller.DeleteWallet(999);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public async Task GetTotalBalance_ValidRequest_ReturnsOkResult()
    {
      // Arrange
      var expectedBalance = new TotalBalanceDTO
      {
        TotalBalance = 1000.00m,
        Wallets = new List<WalletBalanceDTO>
                {
                    new WalletBalanceDTO
                    {
                        Id = TestWallet.Id,
                        Name = TestWallet.Name,
                        Balance = TestWallet.Balance
                    }
                }
      };

      _mockWalletService.Setup(x => x.GetTotalBalanceAsync(TestUser.Id))
          .ReturnsAsync(expectedBalance);

      // Act
      var result = await _controller.GetTotalBalance();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<TotalBalanceDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(expectedBalance.TotalBalance, response.Data.TotalBalance);
      Assert.Single(response.Data.Wallets);
    }

    [Fact]
    public async Task GetWalletTransferInfo_ExistingWallet_ReturnsOkResult()
    {
      // Arrange
      var expectedInfo = new WalletTransferInfoDTO
      {
        WalletId = TestWallet.Id,
        WalletName = TestWallet.Name,
        OwnerName = TestUser.Name,
        CreatedAt = TestWallet.CreatedAt
      };

      _mockWalletService.Setup(x => x.GetWalletTransferInfoAsync(TestWallet.Id))
          .ReturnsAsync(expectedInfo);

      // Act
      var result = await _controller.GetWalletTransferInfo(TestWallet.Id);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<WalletTransferInfoDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(TestWallet.Id, response.Data.WalletId);
    }

    [Fact]
    public async Task GetWalletTransferInfoByEmail_ExistingEmail_ReturnsOkResult()
    {
      // Arrange
      var expectedWallets = new List<WalletTransferInfoDTO>
            {
                new WalletTransferInfoDTO
                {
                    WalletId = TestWallet.Id,
                    WalletName = TestWallet.Name,
                    OwnerName = TestUser.Name,
                    CreatedAt = TestWallet.CreatedAt
                }
            };

      _mockWalletService.Setup(x => x.GetWalletTransferInfoByEmailAsync(TestUser.Email))
          .ReturnsAsync(expectedWallets);

      // Act
      var result = await _controller.GetWalletTransferInfoByEmail(TestUser.Email);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      var response = Assert.IsType<ApiResponse<IEnumerable<WalletTransferInfoDTO>>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Single(response.Data);
    }

    [Fact]
    public async Task GetWalletTransferInfoByCPF_ExistingCPF_ReturnsOkResult()
    {
      // Arrange
      var expectedWallets = new List<WalletTransferInfoDTO>
            {
                new WalletTransferInfoDTO
                {
                    WalletId = TestWallet.Id,
                    WalletName = TestWallet.Name,
                    OwnerName = TestUser.Name,
                    CreatedAt = TestWallet.CreatedAt
                }
            };

      _mockWalletService.Setup(x => x.GetWalletTransferInfoByCPFAsync(TestUser.CPF))
          .ReturnsAsync(expectedWallets);

      // Act
      var result = await _controller.GetWalletTransferInfoByCPF(TestUser.CPF);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      var response = Assert.IsType<ApiResponse<IEnumerable<WalletTransferInfoDTO>>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Single(response.Data);
    }

    [Fact]
    public async Task GetWalletTransferInfoByEmail_NonExistingEmail_ReturnsNotFound()
    {
      // Arrange
      var nonExistingEmail = "nonexisting@example.com";
      _mockWalletService.Setup(x => x.GetWalletTransferInfoByEmailAsync(nonExistingEmail))
          .ThrowsAsync(new AppException("Nenhuma carteira encontrada para o email informado"));

      // Act
      var result = await _controller.GetWalletTransferInfoByEmail(nonExistingEmail);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
      var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public async Task GetWalletTransferInfoByCPF_NonExistingCPF_ReturnsNotFound()
    {
      // Arrange
      var nonExistingCPF = "99999999999";
      _mockWalletService.Setup(x => x.GetWalletTransferInfoByCPFAsync(nonExistingCPF))
          .ThrowsAsync(new AppException("Nenhuma carteira encontrada para o CPF informado"));

      // Act
      var result = await _controller.GetWalletTransferInfoByCPF(nonExistingCPF);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
      var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
      Assert.False(response.Success);
    }
  }
}
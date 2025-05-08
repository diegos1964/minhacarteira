using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.Controllers;
using MinhaCarteira.API.DTOs;
using Moq;
using Xunit;

namespace MinhaCarteira.Tests.Controllers
{
  public class WalletControllerTests : TestBase
  {
    private readonly WalletController _controller;

    public WalletControllerTests()
    {
      _controller = new WalletController(WalletService.Object);

      // Setup controller user claims
      var claims = new List<Claim>
            {
                new Claim("id", TestUser.Id.ToString())
            };
      var identity = new ClaimsIdentity(claims);
      var claimsPrincipal = new ClaimsPrincipal(identity);
      _controller.ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = claimsPrincipal }
      };

      // Setup mock behaviors
      WalletService.Setup(x => x.CreateWalletAsync(It.IsAny<CreateWalletDTO>(), TestUser.Id))
          .ReturnsAsync((CreateWalletDTO dto, int userId) => new WalletDTO
          {
            Id = TestWallet.Id,
            Name = dto.Name,
            Balance = dto.InitialBalance,
            CreatedAt = TestWallet.CreatedAt,
            UpdatedAt = TestWallet.UpdatedAt
          });

      WalletService.Setup(x => x.GetUserWalletsAsync(TestUser.Id))
          .ReturnsAsync(new List<WalletDTO>
          {
                    new WalletDTO
                    {
                        Id = TestWallet.Id,
                        Name = TestWallet.Name,
                        Balance = TestWallet.Balance,
                        CreatedAt = TestWallet.CreatedAt,
                        UpdatedAt = TestWallet.UpdatedAt
                    }
          });

      WalletService.Setup(x => x.GetWalletAsync(TestWallet.Id, TestUser.Id))
          .ReturnsAsync(new WalletDTO
          {
            Id = TestWallet.Id,
            Name = TestWallet.Name,
            Balance = TestWallet.Balance,
            CreatedAt = TestWallet.CreatedAt,
            UpdatedAt = TestWallet.UpdatedAt
          });
    }

    [Fact]
    public async Task CreateWallet_ValidData_ReturnsCreatedResult()
    {
      // Arrange
      var createDto = new CreateWalletDTO
      {
        Name = "Test Wallet",
        InitialBalance = 0
      };

      // Act
      var result = await _controller.CreateWallet(createDto);

      // Assert
      var actionResult = Assert.IsType<ActionResult<WalletDTO>>(result);
      var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var wallet = Assert.IsType<WalletDTO>(createdResult.Value);
      Assert.Equal(createDto.Name, wallet.Name);
      Assert.Equal(createDto.InitialBalance, wallet.Balance);
    }

    [Fact]
    public async Task GetWallets_ReturnsUserWallets()
    {
      // Act
      var result = await _controller.GetWallets();

      // Assert
      var actionResult = Assert.IsType<ActionResult<IEnumerable<WalletDTO>>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var wallets = Assert.IsType<List<WalletDTO>>(okResult.Value);
      Assert.Single(wallets);
      Assert.Equal(TestWallet.Name, wallets[0].Name);
    }

    [Fact]
    public async Task GetWallet_ExistingWallet_ReturnsWallet()
    {
      // Act
      var result = await _controller.GetWallet(TestWallet.Id);

      // Assert
      var actionResult = Assert.IsType<ActionResult<WalletDTO>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var returnedWallet = Assert.IsType<WalletDTO>(okResult.Value);
      Assert.Equal(TestWallet.Id, returnedWallet.Id);
      Assert.Equal(TestWallet.Name, returnedWallet.Name);
    }

    [Fact]
    public async Task GetWallet_NonExistingWallet_ReturnsNotFound()
    {
      // Arrange
      WalletService.Setup(x => x.GetWalletAsync(999, TestUser.Id))
          .ReturnsAsync((WalletDTO)null);

      // Act
      var result = await _controller.GetWallet(999);

      // Assert
      var actionResult = Assert.IsType<ActionResult<WalletDTO>>(result);
      Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task UpdateWallet_ValidData_ReturnsNoContent()
    {
      // Arrange
      var updateDto = new UpdateWalletDTO
      {
        Name = "New Name"
      };

      // Act
      var result = await _controller.UpdateWallet(TestWallet.Id, updateDto);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteWallet_ExistingWallet_ReturnsNoContent()
    {
      // Act
      var result = await _controller.DeleteWallet(TestWallet.Id);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetTotalBalance_ReturnsBalance()
    {
      // Arrange
      WalletService.Setup(x => x.GetTotalBalanceAsync(TestUser.Id))
          .ReturnsAsync(1000m);

      // Act
      var result = await _controller.GetTotalBalance();

      // Assert
      var actionResult = Assert.IsType<ActionResult<decimal>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var balance = Assert.IsType<decimal>(okResult.Value);
      Assert.Equal(1000m, balance);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinhaCarteira.API.Controllers;
using MinhaCarteira.API.DTOs.Reponses;
using MinhaCarteira.API.DTOs.Transaction;
using MinhaCarteira.API.Exceptions;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Services;
using Moq;
using Xunit;

namespace MinhaCarteira.Tests.Controllers
{
  public class TransactionControllerTests : TestBase
  {
    private readonly Mock<ITransactionService> _mockTransactionService;
    private readonly Mock<ILogger<TransactionController>> _mockLogger;
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
      _mockTransactionService = new Mock<ITransactionService>();
      _mockLogger = new Mock<ILogger<TransactionController>>();
      _controller = new TransactionController(_mockTransactionService.Object);

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
    public async Task GetTransactions_ValidRequest_ReturnsOkResult()
    {
      // Arrange
      var filter = new TransactionFilterDTO { PageNumber = 1, PageSize = 10 };
      var expectedResponse = new PaginatedResultDTO<TransactionDTO>
      {
        Items = new List<TransactionDTO>
                {
                    new TransactionDTO
                    {
                        Id = 1,
                        Description = "Test Transaction",
                        Amount = 100m,
                        Type = TransactionType.Income.ToString(),
                        Date = DateTime.UtcNow,
                        WalletId = TestWallet.Id,
                        WalletName = TestWallet.Name,
                        CreatedAt = DateTime.UtcNow
                    }
                },
        TotalItems = 1,
        PageNumber = 1,
        PageSize = 10,
        TotalPages = 1
      };

      _mockTransactionService.Setup(x => x.GetUserTransactionsAsync(TestUser.Id, It.IsAny<TransactionFilterDTO>()))
          .ReturnsAsync(expectedResponse);

      // Act
      var result = await _controller.GetTransactions(filter);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<PaginatedResultDTO<TransactionDTO>>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Single(response.Data.Items);
    }

    [Fact]
    public async Task GetTransaction_ExistingTransaction_ReturnsOkResult()
    {
      // Arrange
      var transactionDto = new TransactionDTO
      {
        Id = 1,
        Description = "Test Transaction",
        Amount = 100m,
        Type = TransactionType.Income.ToString(),
        Date = DateTime.UtcNow,
        WalletId = TestWallet.Id,
        WalletName = TestWallet.Name,
        CreatedAt = DateTime.UtcNow
      };

      _mockTransactionService.Setup(x => x.GetTransactionAsync(1, TestUser.Id))
          .ReturnsAsync(transactionDto);

      // Act
      var result = await _controller.GetTransaction(1);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<TransactionDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(1, response.Data.Id);
    }

    [Fact]
    public async Task GetTransaction_NonExistingTransaction_ReturnsNotFound()
    {
      // Arrange
      _mockTransactionService.Setup(x => x.GetTransactionAsync(It.IsAny<int>(), TestUser.Id))
          .ReturnsAsync((TransactionDTO?)null);

      // Act
      var result = await _controller.GetTransaction(999);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public async Task CreateTransaction_ValidData_ReturnsCreatedResult()
    {
      // Arrange
      var createDto = new CreateTransactionDTO
      {
        Description = "New Transaction",
        Amount = 100m,
        Type = TransactionType.Income.ToString(),
        WalletId = TestWallet.Id,
        Date = DateTime.UtcNow
      };

      var createdTransaction = new TransactionDTO
      {
        Id = 2,
        Description = createDto.Description,
        Amount = createDto.Amount,
        Type = createDto.Type,
        WalletId = createDto.WalletId,
        WalletName = TestWallet.Name,
        Date = createDto.Date,
        CreatedAt = DateTime.UtcNow
      };

      _mockTransactionService.Setup(x => x.CreateTransactionAsync(createDto, TestUser.Id))
          .ReturnsAsync(createdTransaction);

      // Act
      var result = await _controller.CreateTransaction(createDto);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
      var response = Assert.IsType<ApiResponse<TransactionDTO>>(createdResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(createDto.Description, response.Data.Description);
      Assert.Equal(createDto.Amount, response.Data.Amount);
    }

    [Fact]
    public async Task UpdateTransaction_ValidData_ReturnsOkResult()
    {
      // Arrange
      var updateDto = new UpdateTransactionDTO { Description = "Updated Transaction" };

      _mockTransactionService.Setup(x => x.UpdateTransactionAsync(1, updateDto, TestUser.Id))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateTransaction(1, updateDto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
      Assert.True(response.Success);
    }

    [Fact]
    public async Task UpdateTransaction_NonExistingTransaction_ReturnsBadRequest()
    {
      // Arrange
      var updateDto = new UpdateTransactionDTO { Description = "Updated Transaction" };

      _mockTransactionService.Setup(x => x.UpdateTransactionAsync(999, updateDto, TestUser.Id))
          .ThrowsAsync(new InvalidOperationException("Transação não encontrada"));

      // Act
      var result = await _controller.UpdateTransaction(999, updateDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public async Task DeleteTransaction_ExistingTransaction_ReturnsOkResult()
    {
      // Arrange
      _mockTransactionService.Setup(x => x.DeleteTransactionAsync(1, TestUser.Id))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteTransaction(1);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
      Assert.True(response.Success);
    }

    [Fact]
    public async Task DeleteTransaction_NonExistingTransaction_ReturnsBadRequest()
    {
      // Arrange
      _mockTransactionService.Setup(x => x.DeleteTransactionAsync(999, TestUser.Id))
          .ThrowsAsync(new InvalidOperationException("Transação não encontrada"));

      // Act
      var result = await _controller.DeleteTransaction(999);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public async Task CreateTransfer_ValidData_ReturnsCreatedResult()
    {
      // Arrange
      var transferDto = new TransferDTO
      {
        SourceWalletId = TestWallet.Id,
        DestinationWalletId = 2,
        Amount = 100m,
        Description = "Test Transfer"
      };

      var createdTransaction = new TransactionDTO
      {
        Id = 1,
        Description = transferDto.Description,
        Amount = transferDto.Amount,
        Type = TransactionType.Transfer.ToString(),
        WalletId = transferDto.SourceWalletId,
        WalletName = TestWallet.Name,
        Date = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
      };

      _mockTransactionService.Setup(x => x.TransferAsync(TestUser.Id, transferDto))
          .ReturnsAsync(createdTransaction);

      // Act
      var result = await _controller.CreateTransfer(transferDto);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
      var response = Assert.IsType<ApiResponse<TransactionDTO>>(createdResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.Equal(transferDto.Amount, response.Data.Amount);
    }

    [Fact]
    public async Task CreateTransfer_InvalidData_ReturnsBadRequest()
    {
      // Arrange
      var transferDto = new TransferDTO
      {
        SourceWalletId = TestWallet.Id,
        DestinationWalletId = 2,
        Amount = 100m,
        Description = "Test Transfer"
      };

      _mockTransactionService.Setup(x => x.TransferAsync(TestUser.Id, transferDto))
          .ThrowsAsync(new InvalidOperationException("Saldo insuficiente"));

      // Act
      var result = await _controller.CreateTransfer(transferDto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
      Assert.False(response.Success);
    }

    [Fact]
    public void GetTransactionTypes_ReturnsOkResult()
    {
      // Act
      var result = _controller.GetTransactionTypes();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var response = Assert.IsType<ApiResponse<TransactionTypesDTO>>(okResult.Value);
      Assert.True(response.Success);
      Assert.NotNull(response.Data);
      Assert.NotEmpty(response.Data.Types);
      Assert.Contains("Income", response.Data.Types);
      Assert.Contains("Expense", response.Data.Types);
      Assert.Contains("Transfer", response.Data.Types);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.Controllers;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;
using Moq;
using Xunit;

namespace MinhaCarteira.Tests.Controllers
{
  public class TransactionControllerTests : TestBase
  {
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
      _controller = new TransactionController(TransactionService.Object);

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
      var testTransactionDto = new TransactionDTO
      {
        Id = 1,
        Description = "Test Transaction",
        Amount = 100,
        Type = TransactionType.Expense.ToString(),
        WalletId = TestWallet.Id,
        WalletName = TestWallet.Name,
        Date = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
      };

      TransactionService.Setup(x => x.CreateTransactionAsync(It.IsAny<CreateTransactionDTO>(), TestUser.Id))
          .ReturnsAsync(testTransactionDto);

      TransactionService.Setup(x => x.GetUserTransactionsAsync(TestUser.Id, It.IsAny<TransactionFilterDTO>()))
          .ReturnsAsync(new List<TransactionDTO> { testTransactionDto });

      TransactionService.Setup(x => x.GetTransactionAsync(testTransactionDto.Id, TestUser.Id))
          .ReturnsAsync(testTransactionDto);
    }

    [Fact]
    public async Task CreateTransaction_ValidData_ReturnsCreatedResult()
    {
      // Arrange
      var createDto = new CreateTransactionDTO
      {
        Description = "Test Transaction",
        Amount = 100,
        Type = TransactionType.Expense,
        WalletId = TestWallet.Id
      };

      // Act
      var result = await _controller.CreateTransaction(createDto);

      // Assert
      var actionResult = Assert.IsType<ActionResult<TransactionDTO>>(result);
      var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var transaction = Assert.IsType<TransactionDTO>(createdResult.Value);
      Assert.Equal(createDto.Description, transaction.Description);
      Assert.Equal(createDto.Amount, transaction.Amount);
      Assert.Equal(createDto.Type.ToString(), transaction.Type);
    }

    [Fact]
    public async Task GetTransactions_ReturnsUserTransactions()
    {
      // Arrange
      var filter = new TransactionFilterDTO
      {
        StartDate = DateTime.UtcNow.AddDays(-30),
        EndDate = DateTime.UtcNow,
        Type = null,
        WalletId = null
      };

      // Act
      var result = await _controller.GetTransactions(filter);

      // Assert
      var actionResult = Assert.IsType<ActionResult<IEnumerable<TransactionDTO>>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var transactions = Assert.IsType<List<TransactionDTO>>(okResult.Value);
      Assert.Single(transactions);
    }

    [Fact]
    public async Task GetTransaction_ExistingTransaction_ReturnsTransaction()
    {
      // Arrange
      var transactionId = 1;

      // Act
      var result = await _controller.GetTransaction(transactionId);

      // Assert
      var actionResult = Assert.IsType<ActionResult<TransactionDTO>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var returnedTransaction = Assert.IsType<TransactionDTO>(okResult.Value);
      Assert.Equal(transactionId, returnedTransaction.Id);
    }

    [Fact]
    public async Task GetTransaction_NonExistingTransaction_ReturnsNotFound()
    {
      // Arrange
      TransactionService.Setup(x => x.GetTransactionAsync(999, TestUser.Id))
          .ReturnsAsync((TransactionDTO)null);

      // Act
      var result = await _controller.GetTransaction(999);

      // Assert
      var actionResult = Assert.IsType<ActionResult<TransactionDTO>>(result);
      Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task UpdateTransaction_ValidData_ReturnsNoContent()
    {
      // Arrange
      var transactionId = 1;
      var updateDto = new UpdateTransactionDTO
      {
        Description = "Updated Description"
      };

      // Act
      var result = await _controller.UpdateTransaction(transactionId, updateDto);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTransaction_ExistingTransaction_ReturnsNoContent()
    {
      // Arrange
      var transactionId = 1;

      // Act
      var result = await _controller.DeleteTransaction(transactionId);

      // Assert
      Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetTotalIncome_ReturnsAmount()
    {
      // Arrange
      var walletId = 1;
      TransactionService.Setup(x => x.GetTotalIncomeAsync(walletId))
          .ReturnsAsync(500m);

      // Act
      var result = await _controller.GetTotalIncome(walletId);

      // Assert
      var actionResult = Assert.IsType<ActionResult<decimal>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var amount = Assert.IsType<decimal>(okResult.Value);
      Assert.Equal(500m, amount);
    }

    [Fact]
    public async Task GetTotalExpense_ReturnsAmount()
    {
      // Arrange
      var walletId = 1;
      TransactionService.Setup(x => x.GetTotalExpenseAsync(walletId))
          .ReturnsAsync(300m);

      // Act
      var result = await _controller.GetTotalExpense(walletId);

      // Assert
      var actionResult = Assert.IsType<ActionResult<decimal>>(result);
      var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var amount = Assert.IsType<decimal>(okResult.Value);
      Assert.Equal(300m, amount);
    }

    [Fact]
    public async Task CreateTransfer_ValidData_ReturnsCreatedResult()
    {
      // Arrange
      var transferDto = new TransferDTO
      {
        SourceWalletId = TestWallet.Id,
        DestinationWalletId = 2,
        Amount = 100,
        Description = "Test Transfer"
      };

      var transferTransactionDto = new TransactionDTO
      {
        Id = 1,
        Description = $"Transferência para Wallet 2: Test Transfer",
        Amount = 100,
        Type = TransactionType.Transfer.ToString(),
        WalletId = TestWallet.Id,
        WalletName = TestWallet.Name,
        Date = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
      };

      TransactionService.Setup(x => x.CreateTransferAsync(transferDto, TestUser.Id))
          .ReturnsAsync(transferTransactionDto);

      // Act
      var result = await _controller.CreateTransfer(transferDto);

      // Assert
      var actionResult = Assert.IsType<ActionResult<TransactionDTO>>(result);
      var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var transaction = Assert.IsType<TransactionDTO>(createdResult.Value);
      Assert.Equal(transferTransactionDto.Description, transaction.Description);
      Assert.Equal(transferTransactionDto.Amount, transaction.Amount);
      Assert.Equal(transferTransactionDto.Type, transaction.Type);
    }
  }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Services;
using Moq;

namespace MinhaCarteira.Tests
{
  public abstract class TestBase
  {
    protected readonly ApplicationDbContext Context;
    protected readonly User TestUser;
    protected readonly Wallet TestWallet;
    protected readonly Mock<IAuthService> AuthService;
    protected readonly Mock<IWalletService> WalletService;
    protected readonly Mock<ITransactionService> TransactionService;
    protected readonly Mock<IConfiguration> Configuration;

    protected TestBase()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: $"TestDb_{GetType().Name}")
          .Options;

      Context = new ApplicationDbContext(options);

      // Setup test user
      TestUser = new User
      {
        Id = 1,
        Name = "Test User",
        Email = "test@email.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
      };

      // Setup test wallet
      TestWallet = new Wallet
      {
        Id = 1,
        Name = "Test Wallet",
        UserId = TestUser.Id,
        Balance = 1000
      };

      // Setup mocks
      Configuration = new Mock<IConfiguration>();
      Configuration.Setup(x => x["Jwt:Key"]).Returns("your-secret-key-here-min-16-chars");
      Configuration.Setup(x => x["Jwt:Issuer"]).Returns("minhacarteira-api");
      Configuration.Setup(x => x["Jwt:Audience"]).Returns("minhacarteira-client");

      AuthService = new Mock<IAuthService>();
      WalletService = new Mock<IWalletService>();
      TransactionService = new Mock<ITransactionService>();
    }

    protected void Cleanup()
    {
      Context.Database.EnsureDeleted();
    }
  }
}
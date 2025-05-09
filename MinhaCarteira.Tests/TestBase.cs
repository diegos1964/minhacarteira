using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Services;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MinhaCarteira.Tests
{
  public abstract class TestBase
  {
    protected readonly ApplicationDbContext Context;
    protected readonly User TestUser;
    protected readonly ClaimsPrincipal TestUserClaims;
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
      TestUser = new User("Test User", "test@example.com", "password123", "12345678900");
      typeof(User).GetProperty("Id")?.SetValue(TestUser, 1);

      // Setup test wallet
      TestWallet = new Wallet
      {
        Id = 1,
        Name = "Test Wallet",
        Balance = 1000.00m,
        UserId = TestUser.Id,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      // Setup mocks
      Configuration = new Mock<IConfiguration>();
      Configuration.Setup(x => x["Jwt:Key"]).Returns("your-secret-key-here-min-16-chars");
      Configuration.Setup(x => x["Jwt:Issuer"]).Returns("minhacarteira-api");
      Configuration.Setup(x => x["Jwt:Audience"]).Returns("minhacarteira-client");

      AuthService = new Mock<IAuthService>();
      WalletService = new Mock<IWalletService>();
      TransactionService = new Mock<ITransactionService>();

      // Setup test user claims
      var claims = new[]
      {
        new Claim("id", TestUser.Id.ToString()),
        new Claim("email", TestUser.Email),
        new Claim("name", TestUser.Name)
      };

      TestUserClaims = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }

    protected void Cleanup()
    {
      Context.Database.EnsureDeleted();
    }
  }
}
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Models;
using BCrypt.Net;
using MinhaCarteira.API.Data.Seed;

namespace MinhaCarteira.API.Data;

public static class DbSeeder
{
  public static async Task SeedData(ApplicationDbContext context)
  {
    if (await context.Users.AnyAsync())
    {
      return; 
    }

    // Criar usuários
    var users = UserSeedData.Users.ToList();

    await context.Users.AddRangeAsync(users);
    await context.SaveChangesAsync();

    // Criar carteiras para cada usuário
    var wallets = new List<Wallet>();
    foreach (var user in users)
    {
      wallets.Add(new Wallet
      {
        Name = "Carteira Principal",
        UserId = user.Id,
        CreatedAt = DateTime.UtcNow
      });

      wallets.Add(new Wallet
      {
        Name = "Carteira Poupança",
        UserId = user.Id,
        CreatedAt = DateTime.UtcNow
      });
    }

    await context.Wallets.AddRangeAsync(wallets);
    await context.SaveChangesAsync();

    // Criar algumas transações iniciais
    var transactions = new List<Transaction>();
    var random = new Random();

    foreach (var wallet in wallets)
    {
      // Adicionar algumas entradas
      for (int i = 0; i < 3; i++)
      {
        transactions.Add(new Transaction
        {
          Amount = random.Next(100, 1000),
          Description = $"Salário {i + 1}",
          Date = DateTime.UtcNow.AddDays(-i * 7),
          Type = TransactionType.Income,
          WalletId = wallet.Id,
          CreatedAt = DateTime.UtcNow
        });
      }

      // Adicionar algumas saídas
      for (int i = 0; i < 2; i++)
      {
        transactions.Add(new Transaction
        {
          Amount = random.Next(50, 500),
          Description = $"Compras {i + 1}",
          Date = DateTime.UtcNow.AddDays(-i * 5),
          Type = TransactionType.Expense,
          WalletId = wallet.Id,
          CreatedAt = DateTime.UtcNow
        });
      }
    }

    await context.Transactions.AddRangeAsync(transactions);
    await context.SaveChangesAsync();
  }
}
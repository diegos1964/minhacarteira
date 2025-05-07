using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
  public TransactionRepository(ApplicationDbContext context) : base(context)
  {
  }

  public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId)
  {
    return await _dbSet
        .Where(t => t.WalletId == walletId)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();
  }

  public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
  {
    return await _dbSet
        .Include(t => t.Wallet)
        .Where(t => t.Wallet.UserId == userId)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();
  }

  public async Task<decimal> GetTotalIncomeByWalletIdAsync(int walletId)
  {
    return await _dbSet
        .Where(t => t.WalletId == walletId && t.Type == TransactionType.Income)
        .SumAsync(t => t.Amount);
  }

  public async Task<decimal> GetTotalExpenseByWalletIdAsync(int walletId)
  {
    return await _dbSet
        .Where(t => t.WalletId == walletId && t.Type == TransactionType.Expense)
        .SumAsync(t => t.Amount);
  }
}
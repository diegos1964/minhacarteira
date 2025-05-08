using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.DTOs;
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

  public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetUserTransactionsAsync(int userId, TransactionFilterDTO filter)
  {
    filter ??= new TransactionFilterDTO();
    var query = _context.Transactions
        .Include(t => t.Wallet)
        .Where(t => t.Wallet.UserId == userId);

    if (filter.StartDate.HasValue)
      query = query.Where(t => t.Date >= filter.StartDate.Value);

    if (filter.EndDate.HasValue)
      query = query.Where(t => t.Date <= filter.EndDate.Value);

    if (filter.WalletId.HasValue)
      query = query.Where(t => t.WalletId == filter.WalletId.Value);

    if (!string.IsNullOrEmpty(filter.Type))
      query = query.Where(t => t.Type.ToString() == filter.Type);

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderByDescending(t => t.Date)
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return (items, totalCount);
  }

  public override async Task<Transaction> AddAsync(Transaction transaction)
  {
    await _dbSet.AddAsync(transaction);
    await _context.SaveChangesAsync();
    return transaction;
  }

  public async Task UpdateAsync(Transaction transaction)
  {
    _dbSet.Update(transaction);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(Transaction transaction)
  {
    _dbSet.Remove(transaction);
    await _context.SaveChangesAsync();
  }

  public async Task<decimal> GetTotalIncomeAsync(int walletId)
  {
    return await GetTotalIncomeByWalletIdAsync(walletId);
  }

  public async Task<decimal> GetTotalExpenseAsync(int walletId)
  {
    return await GetTotalExpenseByWalletIdAsync(walletId);
  }
}
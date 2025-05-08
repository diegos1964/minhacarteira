using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
{
  public WalletRepository(ApplicationDbContext context) : base(context)
  {
  }

  public async Task<IEnumerable<Wallet>> GetByUserIdAsync(int userId)
  {
    return await _dbSet
        .Where(w => w.UserId == userId)
        .ToListAsync();
  }

  public async Task<Wallet?> GetByIdWithTransactionsAsync(int id)
  {
    return await _dbSet
        .Include(w => w.Transactions)
        .FirstOrDefaultAsync(w => w.Id == id);
  }

  public async Task<decimal> GetTotalBalanceByUserIdAsync(int userId)
  {
    return await _dbSet
        .Where(w => w.UserId == userId)
        .SumAsync(w => w.Balance);
  }

  public async Task<(IEnumerable<Wallet> Items, int TotalCount)> GetUserWalletsAsync(int userId, WalletFilterDTO filter)
  {
    filter ??= new WalletFilterDTO();
    var query = _dbSet.Where(w => w.UserId == userId);

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderByDescending(w => w.CreatedAt)
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return (items, totalCount);
  }
}
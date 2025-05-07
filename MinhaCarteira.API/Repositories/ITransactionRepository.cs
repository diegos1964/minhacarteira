using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
  Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
  Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
  Task<decimal> GetTotalIncomeByWalletIdAsync(int walletId);
  Task<decimal> GetTotalExpenseByWalletIdAsync(int walletId);
}
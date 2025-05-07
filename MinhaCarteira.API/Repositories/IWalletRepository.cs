using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public interface IWalletRepository : IBaseRepository<Wallet>
{
  Task<IEnumerable<Wallet>> GetByUserIdAsync(int userId);
  Task<Wallet?> GetByIdWithTransactionsAsync(int id);
  Task<decimal> GetTotalBalanceByUserIdAsync(int userId);
}
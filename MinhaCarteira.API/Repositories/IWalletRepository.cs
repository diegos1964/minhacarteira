using System.Collections.Generic;
using System.Threading.Tasks;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public interface IWalletRepository : IBaseRepository<Wallet>
{
  Task<IEnumerable<Wallet>> GetByUserIdAsync(int userId);
  Task<Wallet?> GetByIdWithTransactionsAsync(int id);
  Task<decimal> GetTotalBalanceByUserIdAsync(int userId);
  Task<(IEnumerable<Wallet> Items, int TotalCount)> GetUserWalletsAsync(int userId, WalletFilterDTO filter);
  Task<decimal> GetTotalBalanceAsync(int userId);
  Task<Wallet?> GetByIdWithUserAsync(int id);
  Task<Wallet?> GetByIdAndUserIdAsync(int id, int userId);
  Task<IEnumerable<Wallet>> GetByUserEmailAsync(string email);
  Task<IEnumerable<Wallet>> GetByUserCPFAsync(string cpf);
}
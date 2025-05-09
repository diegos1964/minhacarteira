using System.Collections.Generic;
using System.Threading.Tasks;
using MinhaCarteira.API.DTOs.Transaction;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
  Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
  Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
  Task<decimal> GetTotalIncomeByWalletIdAsync(int walletId);
  Task<decimal> GetTotalExpenseByWalletIdAsync(int walletId);
  Task<(IEnumerable<Transaction> Items, int TotalCount)> GetUserTransactionsAsync(int userId, TransactionFilterDTO filter);
  Task UpdateAsync(Transaction transaction);
  Task DeleteAsync(Transaction transaction);
  Task<decimal> GetTotalIncomeAsync(int walletId);
  Task<decimal> GetTotalExpenseAsync(int walletId);
}
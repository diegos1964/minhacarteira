using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Services;

public interface ITransactionService
{
  Task<PaginatedResultDTO<TransactionDTO>> GetUserTransactionsAsync(int userId, TransactionFilterDTO filter);
  Task<TransactionDTO?> GetTransactionAsync(int id, int userId);
  Task<TransactionDTO> CreateTransactionAsync(CreateTransactionDTO createTransactionDto, int userId);
  Task<TransactionDTO> CreateTransferAsync(TransferDTO transferDto, int userId);
  Task UpdateTransactionAsync(int id, UpdateTransactionDTO updateTransactionDto, int userId);
  Task DeleteTransactionAsync(int id, int userId);
  Task<decimal> GetTotalIncomeAsync(int walletId);
  Task<decimal> GetTotalExpenseAsync(int walletId);
}
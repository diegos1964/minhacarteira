using MinhaCarteira.API.DTOs.Reponses;
using MinhaCarteira.API.DTOs.Transaction;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Services;

public interface ITransactionService
{
  Task<PaginatedResultDTO<TransactionDTO>> GetUserTransactionsAsync(int userId, TransactionFilterDTO filter);
  Task<TransactionDTO?> GetTransactionAsync(int id, int userId);
  Task<TransactionDTO> CreateTransactionAsync(CreateTransactionDTO createTransactionDto, int userId);
  Task<TransactionDTO> TransferAsync(int userId, TransferDTO transferDto);
  Task UpdateTransactionAsync(int id, UpdateTransactionDTO updateTransactionDto, int userId);
  Task DeleteTransactionAsync(int id, int userId);
  Task<WalletIncomeDTO> GetTotalIncomeAsync(int walletId);
  Task<WalletExpenseDTO> GetTotalExpenseAsync(int walletId);
}
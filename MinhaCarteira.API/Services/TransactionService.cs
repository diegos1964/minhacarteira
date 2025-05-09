using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.DTOs.Reponses;
using MinhaCarteira.API.DTOs.Transaction;
using MinhaCarteira.API.DTOs.Wallet;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Repositories;

namespace MinhaCarteira.API.Services;

public class TransactionService : ITransactionService
{
  private readonly ApplicationDbContext _context;
  private readonly IWalletService _walletService;
  private readonly ITransactionRepository _transactionRepository;
  private readonly IWalletRepository _walletRepository;

  public TransactionService(ApplicationDbContext context, IWalletService walletService, ITransactionRepository transactionRepository, IWalletRepository walletRepository)
  {
    _context = context;
    _walletService = walletService;
    _transactionRepository = transactionRepository;
    _walletRepository = walletRepository;
  }

  public async Task<PaginatedResultDTO<TransactionDTO>> GetUserTransactionsAsync(int userId, TransactionFilterDTO filter)
  {
    var (transactions, totalCount) = await _transactionRepository.GetUserTransactionsAsync(userId, filter);

    var items = transactions.Select(t => new TransactionDTO
    {
      Id = t.Id,
      Amount = t.Amount,
      Description = t.Description,
      Date = t.Date,
      Type = t.Type.ToString(),
      WalletId = t.WalletId,
      WalletName = t.Wallet.Name,
      CreatedAt = t.CreatedAt,
      UpdatedAt = t.UpdatedAt
    });

    return new PaginatedResultDTO<TransactionDTO>
    {
      Items = items,
      TotalItems = totalCount,
      PageNumber = filter.PageNumber,
      PageSize = filter.PageSize,
      TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
    };
  }

  public async Task<TransactionDTO?> GetTransactionAsync(int id, int userId)
  {
    var transaction = await _transactionRepository.GetByIdAsync(id);
    if (transaction == null)
    {
      return null;
    }

    var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
    if (wallet == null || wallet.UserId != userId)
    {
      return null;
    }

    return new TransactionDTO
    {
      Id = transaction.Id,
      Description = transaction.Description,
      Amount = transaction.Amount,
      Type = transaction.Type.ToString(),
      WalletId = transaction.WalletId,
      WalletName = wallet.Name,
      DestinationWalletId = transaction.DestinationWalletId,
      DestinationWalletName = transaction.DestinationWallet?.Name,
      CreatedAt = transaction.CreatedAt,
      UpdatedAt = transaction.UpdatedAt,
      Date = transaction.Date
    };
  }

  public async Task<TransactionDTO> CreateTransactionAsync(CreateTransactionDTO createTransactionDto, int userId)
  {
    var wallet = await _walletRepository.GetByIdAsync(createTransactionDto.WalletId);
    if (wallet == null || wallet.UserId != userId)
    {
      throw new InvalidOperationException("Carteira não encontrada");
    }

    if (!Enum.TryParse<TransactionType>(createTransactionDto.Type, true, out var transactionType))
    {
      throw new InvalidOperationException("Tipo de transação inválido");
    }

    if (transactionType == TransactionType.Transfer && createTransactionDto.DestinationWalletId.HasValue)
    {
      var destinationWallet = await _walletRepository.GetByIdAsync(createTransactionDto.DestinationWalletId.Value);
      if (destinationWallet == null)
      {
        throw new InvalidOperationException("Carteira de destino não encontrada");
      }

      if (destinationWallet.Id == wallet.Id)
      {
        throw new InvalidOperationException("Não é possível transferir para a mesma carteira");
      }
    }

    var transaction = new Transaction
    {
      Description = createTransactionDto.Description,
      Amount = createTransactionDto.Amount,
      Type = transactionType,
      WalletId = createTransactionDto.WalletId,
      DestinationWalletId = createTransactionDto.DestinationWalletId,
      Date = createTransactionDto.Date.ToUniversalTime()
    };

    await _transactionRepository.AddAsync(transaction);

    switch (transactionType)
    {
      case TransactionType.Income:
        wallet.Balance += createTransactionDto.Amount;
        break;
      case TransactionType.Expense:
        if (wallet.Balance < createTransactionDto.Amount)
        {
          throw new InvalidOperationException("Saldo insuficiente");
        }
        wallet.Balance -= createTransactionDto.Amount;
        break;
      case TransactionType.Transfer:
        if (wallet.Balance < createTransactionDto.Amount)
        {
          throw new InvalidOperationException("Saldo insuficiente");
        }
        wallet.Balance -= createTransactionDto.Amount;
        if (createTransactionDto.DestinationWalletId.HasValue)
        {
          var destinationWallet = await _walletRepository.GetByIdAsync(createTransactionDto.DestinationWalletId.Value);
          if (destinationWallet != null)
          {
            destinationWallet.Balance += createTransactionDto.Amount;
            _walletRepository.Update(destinationWallet);
          }
        }
        break;
    }

    _walletRepository.Update(wallet);
    await _transactionRepository.SaveChangesAsync();

    return new TransactionDTO
    {
      Id = transaction.Id,
      Description = transaction.Description,
      Amount = transaction.Amount,
      Type = transaction.Type.ToString(),
      WalletId = transaction.WalletId,
      WalletName = wallet.Name,
      Date = transaction.Date,
      DestinationWalletId = transaction.DestinationWalletId,
      DestinationWalletName = transaction.DestinationWallet?.Name,
      CreatedAt = transaction.CreatedAt,
      UpdatedAt = transaction.UpdatedAt
    };
  }

  public async Task UpdateTransactionAsync(int id, UpdateTransactionDTO updateTransactionDto, int userId)
  {
    var transaction = await _transactionRepository.GetByIdAsync(id);
    if (transaction == null)
    {
      throw new InvalidOperationException("Transação não encontrada");
    }

    var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
    if (wallet == null || wallet.UserId != userId)
    {
      throw new InvalidOperationException("Carteira não encontrada");
    }

    transaction.Description = updateTransactionDto.Description;
    transaction.UpdatedAt = DateTime.UtcNow;

    _transactionRepository.Update(transaction);
    await _transactionRepository.SaveChangesAsync();
  }

  public async Task DeleteTransactionAsync(int id, int userId)
  {
    var transaction = await _transactionRepository.GetByIdAsync(id);
    if (transaction == null)
    {
      throw new InvalidOperationException("Transação não encontrada");
    }

    var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
    if (wallet == null || wallet.UserId != userId)
    {
      throw new InvalidOperationException("Carteira não encontrada");
    }

    switch (transaction.Type)
    {
      case TransactionType.Income:
        wallet.Balance -= transaction.Amount;
        break;
      case TransactionType.Expense:
        wallet.Balance += transaction.Amount;
        break;
      case TransactionType.Transfer:
        wallet.Balance += transaction.Amount;
        if (transaction.DestinationWalletId.HasValue)
        {
          var destinationWallet = await _walletRepository.GetByIdAsync(transaction.DestinationWalletId.Value);
          if (destinationWallet != null)
          {
            destinationWallet.Balance -= transaction.Amount;
            _walletRepository.Update(destinationWallet);
          }
        }
        break;
    }

    _walletRepository.Update(wallet);
    _transactionRepository.Remove(transaction);
    await _transactionRepository.SaveChangesAsync();
  }

  public async Task<WalletIncomeDTO> GetTotalIncomeAsync(int walletId)
  {
    var wallet = await _walletRepository.GetByIdAsync(walletId);
    if (wallet == null)
    {
      throw new InvalidOperationException("Carteira não encontrada");
    }

    var totalIncome = await _transactionRepository.GetTotalIncomeByWalletIdAsync(walletId);
    return new WalletIncomeDTO
    {
      WalletId = wallet.Id,
      WalletName = wallet.Name,
      TotalIncome = totalIncome
    };
  }

  public async Task<WalletExpenseDTO> GetTotalExpenseAsync(int walletId)
  {
    var wallet = await _walletRepository.GetByIdAsync(walletId);
    if (wallet == null)
    {
      throw new InvalidOperationException("Carteira não encontrada");
    }

    var totalExpense = await _transactionRepository.GetTotalExpenseByWalletIdAsync(walletId);
    return new WalletExpenseDTO
    {
      WalletId = wallet.Id,
      WalletName = wallet.Name,
      TotalExpense = totalExpense
    };
  }

  public async Task<TransactionDTO> TransferAsync(int userId, TransferDTO transferDto)
  {
    // Verificar se a carteira de origem pertence ao usuário
    var sourceWallet = await _walletRepository.GetByIdAndUserIdAsync(transferDto.SourceWalletId, userId);
    if (sourceWallet == null)
      throw new InvalidOperationException("Carteira de origem não encontrada ou não pertence ao usuário");

    // Verificar se a carteira de destino existe
    var destinationWallet = await _walletRepository.GetByIdWithUserAsync(transferDto.DestinationWalletId);
    if (destinationWallet == null)
      throw new InvalidOperationException("Carteira de destino não encontrada");

    // Verificar se há saldo suficiente
    var income = await GetTotalIncomeAsync(sourceWallet.Id);
    var expense = await GetTotalExpenseAsync(sourceWallet.Id);
    var currentBalance = income.TotalIncome - expense.TotalExpense;
    if (currentBalance < transferDto.Amount)
      throw new InvalidOperationException("Saldo insuficiente para realizar a transferência");

    // Criar transação de saída na carteira de origem
    var sourceTransaction = new Transaction
    {
      Amount = transferDto.Amount,
      Description = $"Transferência para {destinationWallet.Name} (Usuário: {destinationWallet.User.Name}): {transferDto.Description}",
      Date = DateTime.UtcNow,
      Type = TransactionType.Expense,
      WalletId = sourceWallet.Id,
      CreatedAt = DateTime.UtcNow
    };

    // Criar transação de entrada na carteira de destino
    var destinationTransaction = new Transaction
    {
      Amount = transferDto.Amount,
      Description = $"Transferência de {sourceWallet.Name} (Usuário: {sourceWallet.User.Name}): {transferDto.Description}",
      Date = DateTime.UtcNow,
      Type = TransactionType.Income,
      WalletId = destinationWallet.Id,
      CreatedAt = DateTime.UtcNow
    };

    await _transactionRepository.AddAsync(sourceTransaction);
    await _transactionRepository.AddAsync(destinationTransaction);
    await _transactionRepository.SaveChangesAsync();

    return new TransactionDTO
    {
      Id = sourceTransaction.Id,
      Amount = sourceTransaction.Amount,
      Description = sourceTransaction.Description,
      Date = sourceTransaction.Date,
      Type = sourceTransaction.Type.ToString(),
      WalletId = sourceTransaction.WalletId,
      WalletName = sourceWallet.Name
    };
  }
}
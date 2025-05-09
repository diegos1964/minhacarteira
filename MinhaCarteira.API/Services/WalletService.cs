using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Repositories;
using MinhaCarteira.API.Exceptions;
using MinhaCarteira.API.DTOs.Wallet;
using MinhaCarteira.API.DTOs.Reponses;

namespace MinhaCarteira.API.Services;

public class WalletService : IWalletService
{
  private readonly IWalletRepository _walletRepository;
  private readonly ITransactionRepository _transactionRepository;

  public WalletService(IWalletRepository walletRepository, ITransactionRepository transactionRepository)
  {
    _walletRepository = walletRepository;
    _transactionRepository = transactionRepository;
  }

  public async Task<PaginatedResultDTO<WalletDTO>> GetUserWalletsAsync(int userId, WalletFilterDTO filter)
  {
    var (wallets, totalCount) = await _walletRepository.GetUserWalletsAsync(userId, filter);

    var items = wallets.Select(w => new WalletDTO
    {
      Id = w.Id,
      Name = w.Name,
      Balance = w.Balance,
      CreatedAt = w.CreatedAt,
      UpdatedAt = w.UpdatedAt
    });

    return new PaginatedResultDTO<WalletDTO>
    {
      Items = items,
      TotalItems = totalCount,
      PageNumber = filter.PageNumber,
      PageSize = filter.PageSize,
      TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
    };
  }

  public async Task<WalletDTO?> GetWalletAsync(int id, int userId)
  {
    var wallet = await _walletRepository.GetByIdWithTransactionsAsync(id);
    if (wallet == null || wallet.UserId != userId)
    {
      return null;
    }

    return new WalletDTO
    {
      Id = wallet.Id,
      Name = wallet.Name,
      Balance = wallet.Balance,
      CreatedAt = wallet.CreatedAt,
      UpdatedAt = wallet.UpdatedAt
    };
  }

  public async Task<WalletDTO> CreateWalletAsync(CreateWalletDTO createWalletDto, int userId)
  {
    var wallet = new Wallet
    {
      Name = createWalletDto.Name,
      UserId = userId,
      Balance = 0
    };

    await _walletRepository.AddAsync(wallet);
    await _walletRepository.SaveChangesAsync();

    if (createWalletDto.InitialBalance.HasValue && createWalletDto.InitialBalance.Value > 0)
    {
      var transaction = new Transaction
      {
        WalletId = wallet.Id,
        Amount = createWalletDto.InitialBalance.Value,
        Type = TransactionType.Income,
        Description = "Saldo inicial da carteira",
        Date = DateTime.UtcNow
      };

      await _transactionRepository.AddAsync(transaction);
      await _transactionRepository.SaveChangesAsync();

      wallet.Balance = createWalletDto.InitialBalance.Value;
      _walletRepository.Update(wallet);
      await _walletRepository.SaveChangesAsync();
    }

    return new WalletDTO
    {
      Id = wallet.Id,
      Name = wallet.Name,
      Balance = wallet.Balance,
      CreatedAt = wallet.CreatedAt,
      UpdatedAt = wallet.UpdatedAt
    };
  }

  public async Task UpdateWalletAsync(int id, UpdateWalletDTO updateWalletDto, int userId)
  {
    var wallet = await _walletRepository.GetByIdAsync(id);
    if (wallet == null || wallet.UserId != userId)
    {
      throw new AppException("Carteira não encontrada");
    }

    wallet.Name = updateWalletDto.Name;
    wallet.UpdatedAt = DateTime.UtcNow;

    _walletRepository.Update(wallet);
    await _walletRepository.SaveChangesAsync();
  }

  public async Task DeleteWalletAsync(int id, int userId)
  {
    var wallet = await _walletRepository.GetByIdAsync(id);
    if (wallet == null || wallet.UserId != userId)
    {
      throw new AppException("Carteira não encontrada");
    }

    _walletRepository.Remove(wallet);
    await _walletRepository.SaveChangesAsync();
  }

  public async Task<TotalBalanceDTO> GetTotalBalanceAsync(int userId)
  {
    var wallets = await _walletRepository.GetUserWalletsAsync(userId, new WalletFilterDTO { PageSize = int.MaxValue });
    var totalBalance = wallets.Item1.Sum(w => w.Balance);

    return new TotalBalanceDTO
    {
      TotalBalance = totalBalance,
      Wallets = wallets.Item1.Select(w => new WalletBalanceDTO
      {
        Id = w.Id,
        Name = w.Name,
        Balance = w.Balance
      }).ToList()
    };
  }

  public async Task<WalletTransferInfoDTO> GetWalletTransferInfoAsync(int walletId)
  {
    var wallet = await _walletRepository.GetByIdWithUserAsync(walletId);
    if (wallet == null)
    {
      throw new AppException("Carteira não encontrada");
    }

    return new WalletTransferInfoDTO
    {
      WalletId = wallet.Id,
      WalletName = wallet.Name,
      OwnerName = wallet.User.Name,
      CreatedAt = wallet.CreatedAt
    };
  }

  public async Task<IEnumerable<WalletTransferInfoDTO>> GetWalletTransferInfoByEmailAsync(string email)
  {
    var wallets = await _walletRepository.GetByUserEmailAsync(email);
    if (!wallets.Any())
    {
      throw new AppException("Nenhuma carteira encontrada para o email informado");
    }

    return wallets.Select(wallet => new WalletTransferInfoDTO
    {
      WalletId = wallet.Id,
      WalletName = wallet.Name,
      OwnerName = wallet.User.Name,

      CreatedAt = wallet.CreatedAt
    });
  }

  public async Task<IEnumerable<WalletTransferInfoDTO>> GetWalletTransferInfoByCPFAsync(string cpf)
  {
    var wallets = await _walletRepository.GetByUserCPFAsync(cpf);
    if (!wallets.Any())
    {
      throw new AppException("Nenhuma carteira encontrada para o CPF informado");
    }

    return wallets.Select(wallet => new WalletTransferInfoDTO
    {
      WalletId = wallet.Id,
      WalletName = wallet.Name,
      OwnerName = wallet.User.Name,
      CreatedAt = wallet.CreatedAt
    });
  }
}
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Repositories;

namespace MinhaCarteira.API.Services;

public class WalletService : IWalletService
{
  private readonly IWalletRepository _walletRepository;

  public WalletService(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;
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
      Balance = createWalletDto.InitialBalance,
      UserId = userId
    };

    await _walletRepository.AddAsync(wallet);
    await _walletRepository.SaveChangesAsync();

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
      throw new InvalidOperationException("Carteira não encontrada");
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
      throw new InvalidOperationException("Carteira não encontrada");
    }

    _walletRepository.Remove(wallet);
    await _walletRepository.SaveChangesAsync();
  }

  public async Task<decimal> GetTotalBalanceAsync(int userId)
  {
    return await _walletRepository.GetTotalBalanceByUserIdAsync(userId);
  }
}
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Services;

public interface IWalletService
{
  Task<PaginatedResultDTO<WalletDTO>> GetUserWalletsAsync(int userId, WalletFilterDTO filter);
  Task<WalletDTO?> GetWalletAsync(int id, int userId);
  Task<WalletDTO> CreateWalletAsync(CreateWalletDTO createWalletDto, int userId);
  Task UpdateWalletAsync(int id, UpdateWalletDTO updateWalletDto, int userId);
  Task DeleteWalletAsync(int id, int userId);
  Task<decimal> GetTotalBalanceAsync(int userId);
  Task<WalletTransferInfoDTO> GetWalletTransferInfoAsync(int walletId);
}
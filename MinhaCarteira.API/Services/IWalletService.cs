using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.DTOs.Reponses;
using MinhaCarteira.API.DTOs.Wallet;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Services;

public interface IWalletService
{
  Task<PaginatedResultDTO<WalletDTO>> GetUserWalletsAsync(int userId, WalletFilterDTO filter);
  Task<WalletDTO?> GetWalletAsync(int id, int userId);
  Task<WalletDTO> CreateWalletAsync(CreateWalletDTO createWalletDto, int userId);
  Task UpdateWalletAsync(int id, UpdateWalletDTO updateWalletDto, int userId);
  Task DeleteWalletAsync(int id, int userId);
  Task<TotalBalanceDTO> GetTotalBalanceAsync(int userId);
  Task<WalletTransferInfoDTO> GetWalletTransferInfoAsync(int walletId);
  Task<IEnumerable<WalletTransferInfoDTO>> GetWalletTransferInfoByEmailAsync(string email);
  Task<IEnumerable<WalletTransferInfoDTO>> GetWalletTransferInfoByCPFAsync(string cpf);
}
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Services;

public interface IWalletService
{
  Task<IEnumerable<WalletDTO>> GetUserWalletsAsync(int userId);
  Task<WalletDTO?> GetWalletAsync(int id, int userId);
  Task<WalletDTO> CreateWalletAsync(CreateWalletDTO createWalletDto, int userId);
  Task UpdateWalletAsync(int id, UpdateWalletDTO updateWalletDto, int userId);
  Task DeleteWalletAsync(int id, int userId);
  Task<decimal> GetTotalBalanceAsync(int userId);
}
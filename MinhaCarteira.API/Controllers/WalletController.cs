using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Services;

namespace MinhaCarteira.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
  private readonly IWalletService _walletService;

  public WalletController(IWalletService walletService)
  {
    _walletService = walletService;
  }

  [HttpGet]
  public async Task<ActionResult<ApiResponse<PaginatedResultDTO<WalletDTO>>>> GetWallets([FromQuery] WalletFilterDTO? filter)
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var wallets = await _walletService.GetUserWalletsAsync(userId, filter ?? new WalletFilterDTO());
    return Ok(ApiResponse<PaginatedResultDTO<WalletDTO>>.CreateSuccess(wallets, "Carteiras recuperadas com sucesso"));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiResponse<WalletDTO>>> GetWallet(int id)
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var wallet = await _walletService.GetWalletAsync(id, userId);
    if (wallet == null)
    {
      return NotFound(ApiResponse<WalletDTO>.CreateError("Carteira não encontrada"));
    }
    return Ok(ApiResponse<WalletDTO>.CreateSuccess(wallet, "Carteira recuperada com sucesso"));
  }

  [HttpPost]
  public async Task<ActionResult<ApiResponse<WalletDTO>>> CreateWallet(CreateWalletDTO createWalletDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var wallet = await _walletService.CreateWalletAsync(createWalletDto, userId);
      return CreatedAtAction(
        nameof(GetWallet),
        new { id = wallet.Id },
        ApiResponse<WalletDTO>.CreateSuccess(wallet, "Carteira criada com sucesso")
      );
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<WalletDTO>.CreateError(ex.Message));
    }
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ApiResponse<object>>> UpdateWallet(int id, UpdateWalletDTO updateWalletDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _walletService.UpdateWalletAsync(id, updateWalletDto, userId);
      return Ok(ApiResponse<object>.CreateSuccess(null, "Carteira atualizada com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<object>.CreateError(ex.Message));
    }
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<ApiResponse<object>>> DeleteWallet(int id)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _walletService.DeleteWalletAsync(id, userId);
      return Ok(ApiResponse<object>.CreateSuccess(null, "Carteira excluída com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<object>.CreateError(ex.Message));
    }
  }

  [HttpGet("balance")]
  public async Task<ActionResult<ApiResponse<decimal>>> GetTotalBalance()
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var balance = await _walletService.GetTotalBalanceAsync(userId);
    return Ok(ApiResponse<decimal>.CreateSuccess(balance, "Saldo total recuperado com sucesso"));
  }
}
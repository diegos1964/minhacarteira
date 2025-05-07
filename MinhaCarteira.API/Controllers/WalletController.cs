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
  public async Task<ActionResult<IEnumerable<WalletDTO>>> GetWallets()
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var wallets = await _walletService.GetUserWalletsAsync(userId);
    return Ok(wallets);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<WalletDTO>> GetWallet(int id)
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var wallet = await _walletService.GetWalletAsync(id, userId);
    if (wallet == null)
    {
      return NotFound();
    }
    return Ok(wallet);
  }

  [HttpPost]
  public async Task<ActionResult<WalletDTO>> CreateWallet(CreateWalletDTO createWalletDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var wallet = await _walletService.CreateWalletAsync(createWalletDto, userId);
      return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id }, wallet);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateWallet(int id, UpdateWalletDTO updateWalletDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _walletService.UpdateWalletAsync(id, updateWalletDto, userId);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteWallet(int id)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _walletService.DeleteWalletAsync(id, userId);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpGet("balance")]
  public async Task<ActionResult<decimal>> GetTotalBalance()
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var balance = await _walletService.GetTotalBalanceAsync(userId);
    return Ok(balance);
  }
}
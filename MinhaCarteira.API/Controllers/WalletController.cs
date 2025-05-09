using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Services;
using MinhaCarteira.API.Exceptions;
using Microsoft.Extensions.Logging;
using MinhaCarteira.API.DTOs.Wallet;
using MinhaCarteira.API.DTOs.Reponses;

namespace MinhaCarteira.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
  private readonly IWalletService _walletService;
  private readonly ILogger<WalletController> _logger;

  public WalletController(IWalletService walletService, ILogger<WalletController> logger)
  {
    _walletService = walletService;
    _logger = logger;
  }

  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<PaginatedResultDTO<WalletDTO>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<PaginatedResultDTO<WalletDTO>>>> GetWallets([FromQuery] WalletFilterDTO? filter)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var wallets = await _walletService.GetUserWalletsAsync(userId, filter ?? new WalletFilterDTO());
      return Ok(ApiResponse<PaginatedResultDTO<WalletDTO>>.CreateSuccess(wallets, "Carteiras recuperadas com sucesso"));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(ApiResponse<WalletDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<WalletDTO>>> GetWallet(int id)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var wallet = await _walletService.GetWalletAsync(id, userId);
      if (wallet == null)
      {
        return NotFound(ApiResponse<object>.CreateError("Carteira não encontrada"));
      }
      return Ok(ApiResponse<WalletDTO>.CreateSuccess(wallet, "Carteira recuperada com sucesso"));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpPost]
  [ProducesResponseType(typeof(ApiResponse<WalletDTO>), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
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
    catch (AppException ex)
    {
      return BadRequest(ApiResponse<object>.CreateError(ex.Message));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpPatch("{id}")]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
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
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpDelete("{id}")]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
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
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpGet("balance")]
  [ProducesResponseType(typeof(ApiResponse<TotalBalanceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<TotalBalanceDTO>>> GetTotalBalance()
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var balance = await _walletService.GetTotalBalanceAsync(userId);
      return Ok(ApiResponse<TotalBalanceDTO>.CreateSuccess(balance, "Saldo total recuperado com sucesso"));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpGet("transfer-info/{walletId}")]
  [ProducesResponseType(typeof(ApiResponse<WalletTransferInfoDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<WalletTransferInfoDTO>>> GetWalletTransferInfo(int walletId)
  {
    try
    {
      var walletInfo = await _walletService.GetWalletTransferInfoAsync(walletId);
      return Ok(ApiResponse<WalletTransferInfoDTO>.CreateSuccess(walletInfo, "Informações da carteira recuperadas com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<object>.CreateError(ex.Message));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpGet("transfer-info/email/{email}")]
  [ProducesResponseType(typeof(ApiResponse<IEnumerable<WalletTransferInfoDTO>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetWalletTransferInfoByEmail(string email)
  {
    try
    {
      var wallets = await _walletService.GetWalletTransferInfoByEmailAsync(email);
      return Ok(ApiResponse<IEnumerable<WalletTransferInfoDTO>>.CreateSuccess(wallets));
    }
    catch (AppException ex)
    {
      return NotFound(ApiResponse<object>.CreateError(ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Erro ao buscar carteiras por email");
      return StatusCode(500, ApiResponse<IEnumerable<WalletTransferInfoDTO>>.CreateError("Erro interno do servidor"));
    }
  }

  [HttpGet("transfer-info/cpf/{cpf}")]
  [ProducesResponseType(typeof(ApiResponse<IEnumerable<WalletTransferInfoDTO>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetWalletTransferInfoByCPF(string cpf)
  {
    try
    {
      var wallets = await _walletService.GetWalletTransferInfoByCPFAsync(cpf);
      return Ok(ApiResponse<IEnumerable<WalletTransferInfoDTO>>.CreateSuccess(wallets));
    }
    catch (AppException ex)
    {
      return NotFound(ApiResponse<object>.CreateError(ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Erro ao buscar carteiras por CPF");
      return StatusCode(500, ApiResponse<object>.CreateError("Erro interno do servidor"));
    }
  }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.Services;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.DTOs.Transaction;
using MinhaCarteira.API.DTOs.Reponses;

namespace MinhaCarteira.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
  private readonly ITransactionService _transactionService;

  public TransactionController(ITransactionService transactionService)
  {
    _transactionService = transactionService;
  }

  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<PaginatedResultDTO<TransactionDTO>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<PaginatedResultDTO<TransactionDTO>>>> GetTransactions([FromQuery] TransactionFilterDTO? filter)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var transactions = await _transactionService.GetUserTransactionsAsync(userId, filter ?? new TransactionFilterDTO());
      return Ok(ApiResponse<PaginatedResultDTO<TransactionDTO>>.CreateSuccess(transactions, "Transações recuperadas com sucesso"));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(ApiResponse<TransactionDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<TransactionDTO>>> GetTransaction(int id)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var transaction = await _transactionService.GetTransactionAsync(id, userId);
      if (transaction == null)
      {
        return NotFound(ApiResponse<object>.CreateError("Transação não encontrada"));
      }
      return Ok(ApiResponse<TransactionDTO>.CreateSuccess(transaction, "Transação recuperada com sucesso"));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [HttpPost]
  [ProducesResponseType(typeof(ApiResponse<TransactionDTO>), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<TransactionDTO>>> CreateTransaction([FromBody] CreateTransactionDTO createTransactionDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var transaction = await _transactionService.CreateTransactionAsync(createTransactionDto, userId);
      return CreatedAtAction(
          nameof(GetTransaction),
          new { id = transaction.Id },
          ApiResponse<TransactionDTO>.CreateSuccess(transaction, "Transação criada com sucesso")
      );
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

  [HttpPatch("{id}")]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<object>>> UpdateTransaction(int id, [FromBody] UpdateTransactionDTO updateTransactionDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _transactionService.UpdateTransactionAsync(id, updateTransactionDto, userId);
      return Ok(ApiResponse<object>.CreateSuccess(null, "Transação atualizada com sucesso"));
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
  public async Task<ActionResult<ApiResponse<object>>> DeleteTransaction(int id)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _transactionService.DeleteTransactionAsync(id, userId);
      return Ok(ApiResponse<object>.CreateSuccess(null, "Transação excluída com sucesso"));
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

  [HttpGet("wallet/{walletId}/income")]
  [ProducesResponseType(typeof(ApiResponse<WalletIncomeDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<WalletIncomeDTO>>> GetTotalIncome(int walletId)
  {
    try
    {
      var income = await _transactionService.GetTotalIncomeAsync(walletId);
      return Ok(ApiResponse<WalletIncomeDTO>.CreateSuccess(income, "Receita total recuperada com sucesso"));
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

  [HttpGet("wallet/{walletId}/expense")]
  [ProducesResponseType(typeof(ApiResponse<WalletExpenseDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<WalletExpenseDTO>>> GetTotalExpense(int walletId)
  {
    try
    {
      var expense = await _transactionService.GetTotalExpenseAsync(walletId);
      return Ok(ApiResponse<WalletExpenseDTO>.CreateSuccess(expense, "Despesa total recuperada com sucesso"));
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

  [HttpPost("transfer")]
  [ProducesResponseType(typeof(ApiResponse<TransactionDTO>), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<TransactionDTO>>> CreateTransfer(TransferDTO transferDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var transaction = await _transactionService.TransferAsync(userId, transferDto);
      return CreatedAtAction(
          nameof(GetTransaction),
          new { id = transaction.Id },
          ApiResponse<TransactionDTO>.CreateSuccess(transaction, "Transferência realizada com sucesso")
      );
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

  [HttpGet("types")]
  [ProducesResponseType(typeof(ApiResponse<TransactionTypesDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public ActionResult<ApiResponse<TransactionTypesDTO>> GetTransactionTypes()
  {
    try
    {
      var types = new TransactionTypesDTO
      {
        Types = Enum.GetNames(typeof(TransactionType))
      };
      return Ok(ApiResponse<TransactionTypesDTO>.CreateSuccess(types, "Tipos de transação recuperados com sucesso"));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Services;

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
  public async Task<ActionResult<ApiResponse<PaginatedResultDTO<TransactionDTO>>>> GetTransactions([FromQuery] TransactionFilterDTO? filter)
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var transactions = await _transactionService.GetUserTransactionsAsync(userId, filter ?? new TransactionFilterDTO());
    return Ok(ApiResponse<PaginatedResultDTO<TransactionDTO>>.CreateSuccess(transactions, "Transações recuperadas com sucesso"));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiResponse<TransactionDTO>>> GetTransaction(int id)
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var transaction = await _transactionService.GetTransactionAsync(id, userId);
    if (transaction == null)
    {
      return NotFound(ApiResponse<TransactionDTO>.CreateError("Transação não encontrada"));
    }
    return Ok(ApiResponse<TransactionDTO>.CreateSuccess(transaction, "Transação recuperada com sucesso"));
  }

  [HttpPost]
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
      return BadRequest(ApiResponse<TransactionDTO>.CreateError(ex.Message));
    }
  }

  [HttpPut("{id}")]
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
  }

  [HttpDelete("{id}")]
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
  }

  [HttpGet("wallet/{walletId}/income")]
  public async Task<ActionResult<ApiResponse<WalletIncomeDTO>>> GetTotalIncome(int walletId)
  {
    try
    {
      var income = await _transactionService.GetTotalIncomeAsync(walletId);
      return Ok(ApiResponse<WalletIncomeDTO>.CreateSuccess(income, "Receita total recuperada com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<WalletIncomeDTO>.CreateError(ex.Message));
    }
  }

  [HttpGet("wallet/{walletId}/expense")]
  public async Task<ActionResult<ApiResponse<WalletExpenseDTO>>> GetTotalExpense(int walletId)
  {
    try
    {
      var expense = await _transactionService.GetTotalExpenseAsync(walletId);
      return Ok(ApiResponse<WalletExpenseDTO>.CreateSuccess(expense, "Despesa total recuperada com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<WalletExpenseDTO>.CreateError(ex.Message));
    }
  }

  [HttpPost("transfer")]
  public async Task<ActionResult<TransactionDTO>> CreateTransfer(TransferDTO transferDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var transaction = await _transactionService.CreateTransferAsync(transferDto, userId);
      return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }
}
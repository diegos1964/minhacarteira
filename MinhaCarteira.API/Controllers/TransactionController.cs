using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.Services;

namespace MinhaCarteira.API.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
  private readonly ITransactionService _transactionService;

  public TransactionController(ITransactionService transactionService)
  {
    _transactionService = transactionService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactions([FromQuery] TransactionFilterDTO? filter)
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var transactions = await _transactionService.GetUserTransactionsAsync(userId, filter);
    return Ok(transactions);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TransactionDTO>> GetTransaction(int id)
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var transaction = await _transactionService.GetTransactionAsync(id, userId);
    if (transaction == null)
    {
      return NotFound();
    }
    return Ok(transaction);
  }

  [HttpPost]
  public async Task<ActionResult<TransactionDTO>> CreateTransaction(CreateTransactionDTO createTransactionDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var transaction = await _transactionService.CreateTransactionAsync(createTransactionDto, userId);
      return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTransaction(int id, UpdateTransactionDTO updateTransactionDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _transactionService.UpdateTransactionAsync(id, updateTransactionDto, userId);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTransaction(int id)
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      await _transactionService.DeleteTransactionAsync(id, userId);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpGet("wallet/{walletId}/income")]
  public async Task<ActionResult<decimal>> GetTotalIncome(int walletId)
  {
    var income = await _transactionService.GetTotalIncomeAsync(walletId);
    return Ok(income);
  }

  [HttpGet("wallet/{walletId}/expense")]
  public async Task<ActionResult<decimal>> GetTotalExpense(int walletId)
  {
    var expense = await _transactionService.GetTotalExpenseAsync(walletId);
    return Ok(expense);
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
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs;

public class TransactionFilterDTO
{
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public int? WalletId { get; set; }
  public string? Type { get; set; } // "income" ou "expense"
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}
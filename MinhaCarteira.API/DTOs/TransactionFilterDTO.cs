using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs;

public class TransactionFilterDTO
{
  private DateTime? _startDate;
  private DateTime? _endDate;

  public DateTime? StartDate
  {
    get => _startDate;
    set => _startDate = value?.ToUniversalTime();
  }

  public DateTime? EndDate
  {
    get => _endDate;
    set => _endDate = value?.ToUniversalTime();
  }

  public int? WalletId { get; set; }
  public string? Type { get; set; } // "income" ou "expense"
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}
using System.ComponentModel.DataAnnotations;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.DTOs;

public class CreateTransactionDTO
{
  [Required]
  [StringLength(200)]
  public string Description { get; set; } = string.Empty;

  [Required]
  [Range(0.01, double.MaxValue)]
  public decimal Amount { get; set; }

  [Required]
  public TransactionType Type { get; set; }

  [Required]
  public int WalletId { get; set; }

  public int? DestinationWalletId { get; set; }
}

public class UpdateTransactionDTO
{
  [Required]
  [StringLength(200)]
  public string Description { get; set; } = string.Empty;
}

public class TransactionDTO
{
  public int Id { get; set; }
  public string Description { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public string Type { get; set; } = string.Empty;
  public DateTime Date { get; set; }
  public int WalletId { get; set; }
  public string WalletName { get; set; } = string.Empty;
  public int? DestinationWalletId { get; set; }
  public string? DestinationWalletName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs.Transaction;

public class TransferDTO
{
  [Required]
  public int SourceWalletId { get; set; }

  [Required]
  public int DestinationWalletId { get; set; }

  [Required]
  [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
  public decimal Amount { get; set; }

  public string? Description { get; set; }
}
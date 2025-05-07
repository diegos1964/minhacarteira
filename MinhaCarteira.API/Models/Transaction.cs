using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaCarteira.API.Models;

public enum TransactionType
{
  Income,
  Expense,
  Transfer
}

public class Transaction
{
  [Key]
  public int Id { get; set; }

  [Required]
  [StringLength(200)]
  public string Description { get; set; } = string.Empty;

  [Required]
  [Column(TypeName = "decimal(18,2)")]
  public decimal Amount { get; set; }

  [Required]
  public TransactionType Type { get; set; }

  [Required]
  public DateTime Date { get; set; }

  [Required]
  public int WalletId { get; set; }
  public virtual Wallet Wallet { get; set; } = null!;

  public int? DestinationWalletId { get; set; }
  public virtual Wallet? DestinationWallet { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
}
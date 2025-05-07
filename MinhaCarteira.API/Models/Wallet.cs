using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaCarteira.API.Models;

public class Wallet
{
  public int Id { get; set; }

  [Required]
  [StringLength(100)]
  public string Name { get; set; } = string.Empty;

  [Required]
  [Column(TypeName = "decimal(18,2)")]
  public decimal Balance { get; set; }

  [Required]
  public int UserId { get; set; }
  public virtual User User { get; set; } = null!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
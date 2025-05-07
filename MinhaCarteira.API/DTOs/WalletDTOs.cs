using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs;

public class CreateWalletDTO
{
  [Required]
  [StringLength(100)]
  public string Name { get; set; } = string.Empty;

  [Required]
  [Range(0, double.MaxValue)]
  public decimal InitialBalance { get; set; }
}

public class UpdateWalletDTO
{
  [Required]
  [StringLength(100)]
  public string Name { get; set; } = string.Empty;
}

public class WalletDTO
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public decimal Balance { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
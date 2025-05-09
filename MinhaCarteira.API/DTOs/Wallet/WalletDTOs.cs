using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs.Wallet;

public class WalletDTO
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public decimal Balance { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}


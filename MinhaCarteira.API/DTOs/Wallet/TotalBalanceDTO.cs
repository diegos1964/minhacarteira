namespace MinhaCarteira.API.DTOs;

public class TotalBalanceDTO
{
  public decimal TotalBalance { get; set; }
  public List<WalletBalanceDTO> Wallets { get; set; } = new();
}

public class WalletBalanceDTO
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public decimal Balance { get; set; }
}
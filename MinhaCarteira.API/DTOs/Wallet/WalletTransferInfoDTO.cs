public class WalletTransferInfoDTO
{
  public int WalletId { get; set; }
  public string WalletName { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; }
}
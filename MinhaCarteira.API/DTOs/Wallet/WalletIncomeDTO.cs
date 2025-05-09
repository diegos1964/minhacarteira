namespace MinhaCarteira.API.DTOs.Wallet
{
    public class WalletIncomeDTO
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
    }
}

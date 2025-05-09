namespace MinhaCarteira.API.DTOs.Wallet
{
    public class WalletExpenseDTO
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; } = string.Empty;
        public decimal TotalExpense { get; set; }
    }
}

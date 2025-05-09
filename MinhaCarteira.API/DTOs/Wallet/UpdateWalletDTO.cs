using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs.Wallet
{
    public class UpdateWalletDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}

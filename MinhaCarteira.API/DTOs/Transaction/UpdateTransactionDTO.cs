using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs.Transaction
{

    public class UpdateTransactionDTO
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
    }
}

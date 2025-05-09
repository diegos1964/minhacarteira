using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs.Transaction
{
    public class CreateTransactionDTO
    {
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório")]
        [SwaggerSchema(Description = "Tipo da transação: Income (Receita), Expense (Despesa), Transfer (Transferência)")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "A carteira é obrigatória")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "A data da transação é obrigatória")]
        public DateTime Date { get; set; }

        public int? DestinationWalletId { get; set; }
    }
}

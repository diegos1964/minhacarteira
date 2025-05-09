using System.ComponentModel.DataAnnotations;
using MinhaCarteira.API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MinhaCarteira.API.DTOs.Transaction;




public class TransactionDTO
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public int? DestinationWalletId { get; set; }
    public string? DestinationWalletName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}



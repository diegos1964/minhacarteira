using System.ComponentModel.DataAnnotations;

public class CreateWalletDTO
{
  [Required(ErrorMessage = "O nome da carteira é obrigatório")]
  [StringLength(100, ErrorMessage = "O nome da carteira deve ter no máximo 100 caracteres")]
  public string Name { get; set; } = string.Empty;



  /// <summary>
  /// Saldo inicial opcional da carteira. Use apenas para migração de dados ou ajustes.
  /// </summary>
  [Range(0, double.MaxValue, ErrorMessage = "O saldo inicial não pode ser negativo")]
  public decimal? InitialBalance { get; set; }
}
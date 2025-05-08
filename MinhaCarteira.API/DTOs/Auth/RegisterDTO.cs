using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs.Auth;

public class RegisterDTO
{
  [Required(ErrorMessage = "O nome é obrigatório")]
  [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
  public string Name { get; set; } = string.Empty;

  [Required(ErrorMessage = "O email é obrigatório")]
  [EmailAddress(ErrorMessage = "Email inválido")]
  [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "O CPF é obrigatório")]
  [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter 11 dígitos")]
  [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter apenas números")]
  public string CPF { get; set; } = string.Empty;

  [Required(ErrorMessage = "A senha é obrigatória")]
  [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
  public string Password { get; set; } = string.Empty;

  [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
  [Compare("Password", ErrorMessage = "As senhas não conferem")]
  public string ConfirmPassword { get; set; } = string.Empty;
}


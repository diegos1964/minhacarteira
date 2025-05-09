using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MinhaCarteira.API.Validations;

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
  [CPF(ErrorMessage = "CPF inválido")]
  public string CPF { get; set; } = string.Empty;

  [Required(ErrorMessage = "A senha é obrigatória")]
  [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres")]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
    ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial")]
  public string Password { get; set; } = string.Empty;

  [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
  [Compare("Password", ErrorMessage = "As senhas não conferem")]
  public string ConfirmPassword { get; set; } = string.Empty;
}


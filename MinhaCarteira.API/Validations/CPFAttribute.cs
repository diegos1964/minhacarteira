using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MinhaCarteira.API.Validations
{
  public class CPFAttribute : ValidationAttribute
  {
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
      if (value == null)
        return ValidationResult.Success;

      var cpf = value.ToString()?.Replace(".", "").Replace("-", "");

      if (string.IsNullOrEmpty(cpf) || cpf.Length != 11 || !cpf.All(char.IsDigit))
        return new ValidationResult("CPF inválido");

      // Verifica se todos os dígitos são iguais
      if (cpf.Distinct().Count() == 1)
        return new ValidationResult("CPF inválido");

      // Validação do primeiro dígito verificador
      var soma = 0;
      for (int i = 0; i < 9; i++)
        soma += (cpf[i] - '0') * (10 - i);

      var resto = soma % 11;
      var digito1 = resto < 2 ? 0 : 11 - resto;

      if (digito1 != (cpf[9] - '0'))
        return new ValidationResult("CPF inválido");

      // Validação do segundo dígito verificador
      soma = 0;
      for (int i = 0; i < 10; i++)
        soma += (cpf[i] - '0') * (11 - i);

      resto = soma % 11;
      var digito2 = resto < 2 ? 0 : 11 - resto;

      if (digito2 != (cpf[10] - '0'))
        return new ValidationResult("CPF inválido");

      return ValidationResult.Success;
    }
  }
}
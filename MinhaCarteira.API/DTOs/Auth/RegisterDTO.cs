using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.DTOs.Auth;

public class RegisterDTO
{
  [Required]
  [StringLength(100)]
  public string Name { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  [StringLength(100)]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(100, MinimumLength = 6)]
  public string Password { get; set; } = string.Empty;
}


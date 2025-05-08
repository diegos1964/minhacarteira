using System;

namespace MinhaCarteira.API.Services
{
  public class BCryptPasswordHasher : IPasswordHasher
  {
    public string HashPassword(string password)
    {
      if (string.IsNullOrEmpty(password))
        throw new ArgumentNullException(nameof(password));

      return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
      if (string.IsNullOrEmpty(password))
        throw new ArgumentNullException(nameof(password));
      if (string.IsNullOrEmpty(hashedPassword))
        throw new ArgumentNullException(nameof(hashedPassword));

      return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
  }
}
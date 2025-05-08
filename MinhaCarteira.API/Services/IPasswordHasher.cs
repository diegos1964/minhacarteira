using System.Threading.Tasks;

namespace MinhaCarteira.API.Services
{
  public interface IPasswordHasher
  {
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
  }
}
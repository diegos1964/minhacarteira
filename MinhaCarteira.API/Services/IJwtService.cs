using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Services;

public interface IJwtService
{
  string GenerateToken(User user);
}
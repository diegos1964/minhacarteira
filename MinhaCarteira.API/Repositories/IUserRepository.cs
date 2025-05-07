using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
  Task<User?> GetByEmailAsync(string email);
  Task<User?> GetByIdWithWalletsAsync(int id);
}
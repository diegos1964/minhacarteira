using System.Linq.Expressions;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
  Task<User?> GetByEmailAsync(string email);
  Task<User?> GetByIdWithWalletsAsync(int id);
  Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate);
}
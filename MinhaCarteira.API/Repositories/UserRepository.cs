using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.Models;
using System.Linq.Expressions;

namespace MinhaCarteira.API.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
  public UserRepository(ApplicationDbContext context) : base(context)
  {
  }

  public async Task<User?> GetByEmailAsync(string email)
  {
    return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
  }

  public async Task<User?> GetByCPFAsync(string cpf)
  {
    return await _dbSet.FirstOrDefaultAsync(u => u.CPF == cpf);
  }

  public async Task<User?> GetByIdWithWalletsAsync(int id)
  {
    return await _dbSet
        .Include(u => u.Wallets)
        .FirstOrDefaultAsync(u => u.Id == id);
  }

  public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
  {
    return await _dbSet.FirstOrDefaultAsync(predicate);
  }
}
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.Models;

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

  public async Task<User?> GetByIdWithWalletsAsync(int id)
  {
    return await _dbSet
        .Include(u => u.Wallets)
        .FirstOrDefaultAsync(u => u.Id == id);
  }
}
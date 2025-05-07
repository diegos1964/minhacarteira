using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users { get; set; }
  public DbSet<Wallet> Wallets { get; set; }
  public DbSet<Transaction> Transactions { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
      entity.HasIndex(e => e.Email).IsUnique();
    });

    modelBuilder.Entity<Wallet>(entity =>
    {
      entity.HasOne(w => w.User)
              .WithMany(u => u.Wallets)
              .HasForeignKey(w => w.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Transaction>(entity =>
    {
      entity.HasOne(t => t.Wallet)
              .WithMany(w => w.Transactions)
              .HasForeignKey(t => t.WalletId)
              .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(t => t.DestinationWallet)
              .WithMany()
              .HasForeignKey(t => t.DestinationWalletId)
              .OnDelete(DeleteBehavior.Restrict);
    });
  }
}
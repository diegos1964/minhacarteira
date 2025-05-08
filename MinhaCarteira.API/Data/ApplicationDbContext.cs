using Microsoft.EntityFrameworkCore;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users { get; set; } = null!;
  public DbSet<Wallet> Wallets { get; set; } = null!;
  public DbSet<Transaction> Transactions { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
      entity.Property(e => e.PasswordHash).IsRequired();
      entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
      entity.HasIndex(e => e.Email).IsUnique();
    });

    modelBuilder.Entity<Wallet>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Balance).HasPrecision(18, 2);
      entity.HasOne(e => e.User)
              .WithMany(u => u.Wallets)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Transaction>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Amount).HasPrecision(18, 2);
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.Date).IsRequired();
      entity.HasOne(e => e.Wallet)
              .WithMany(w => w.Transactions)
              .HasForeignKey(e => e.WalletId)
              .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(e => e.DestinationWallet)
              .WithMany()
              .HasForeignKey(e => e.DestinationWalletId)
              .OnDelete(DeleteBehavior.Restrict);
    });
  }
}
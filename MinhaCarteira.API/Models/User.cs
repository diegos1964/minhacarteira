using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.API.Models;

public class User
{
    private readonly List<Wallet> _wallets = new();

    public int Id { get; private set; }

    [Required]
    [StringLength(100)]
    public string Name { get; private set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; private set; } = string.Empty;

    [Required]
    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public virtual IReadOnlyCollection<Wallet> Wallets => _wallets.AsReadOnly();

    private User() { }

    public User(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddWallet(Wallet wallet)
    {
        if (wallet == null)
            throw new ArgumentNullException(nameof(wallet));

        _wallets.Add(wallet);
    }

    public void RemoveWallet(Wallet wallet)
    {
        if (wallet == null)
            throw new ArgumentNullException(nameof(wallet));

        _wallets.Remove(wallet);
    }
}
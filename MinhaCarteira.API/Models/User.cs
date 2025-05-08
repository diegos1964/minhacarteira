using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaCarteira.API.Models;

public class User
{
    private readonly List<Wallet> _wallets = new();

    public int Id { get; private set; }

    [Required]
    [StringLength(100)]
    public string Name { get; private set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; private set; } = string.Empty;

    [Required]
    [StringLength(11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 dígitos numéricos")]
    public string CPF { get; private set; } = string.Empty;

    [Required]
    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public virtual IReadOnlyCollection<Wallet> Wallets => _wallets.AsReadOnly();

    private User() { }

    public User(string name, string email, string passwordHash, string cpf)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        if (!string.IsNullOrWhiteSpace(cpf) && !System.Text.RegularExpressions.Regex.IsMatch(cpf, @"^\d{11}$"))
            throw new ArgumentException("CPF must contain exactly 11 digits", nameof(cpf));

        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CPF = cpf;
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

    public void UpdateCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF cannot be empty", nameof(cpf));
        if (!System.Text.RegularExpressions.Regex.IsMatch(cpf, @"^\d{11}$"))
            throw new ArgumentException("CPF must contain exactly 11 digits", nameof(cpf));

        CPF = cpf;
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
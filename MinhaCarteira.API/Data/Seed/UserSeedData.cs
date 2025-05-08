using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Data.Seed;

public static class UserSeedData
{
    public static readonly User[] Users = new[]
    {
        CreateUser(1, "João Silva", "joao@email.com", "12345678901", "123456"),
        CreateUser(2, "Maria Santos", "maria@email.com", "98765432109", "123456"),
        CreateUser(3, "Pedro Oliveira", "pedro@email.com", "45678912345", "123456")
    };

    private static User CreateUser(int id, string name, string email, string cpf, string password)
    {
        var user = new User(name, email, BCrypt.Net.BCrypt.HashPassword(password), cpf);
        typeof(User).GetProperty("Id")?.SetValue(user, id);
        return user;
    }
}
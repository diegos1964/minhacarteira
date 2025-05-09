using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Data.Seed;

public static class UserSeedData
{
    public static readonly User[] Users = new[]
    {
        CreateUser(1, "João Silva", "joao@email.com", "70527849049", "123456@aA"),
        CreateUser(2, "Maria Santos", "maria@email.com", "49757677086", "123456@aA"),
        CreateUser(3, "Pedro Oliveira", "pedro@email.com", "18276884083", "123456@aA")
    };

    private static User CreateUser(int id, string name, string email, string cpf, string password)
    {
        var user = new User(name, email, BCrypt.Net.BCrypt.HashPassword(password), cpf);
        typeof(User).GetProperty("Id")?.SetValue(user, id);
        return user;
    }
}
namespace WebApplication3.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = new();

    public User()
    {
        Id = Guid.NewGuid();
    }

    public User(string login, string password)
    {
        Id = Guid.NewGuid();
        Login = login;
        Password = password;
    }
}
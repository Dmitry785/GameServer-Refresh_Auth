namespace WebApplication3.Domain.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime ExpirateAt { get; set; }
    public bool IsExpired { get; set; }

    public RefreshToken()
    {
        Id = Guid.NewGuid();
    }

    public RefreshToken(Guid userId, DateTime expirateAt)
    {
        Id = Guid.NewGuid();
        Token = Guid.NewGuid().ToString();
        UserId = userId;
        ExpirateAt = expirateAt;
        IsExpired = false;
    }

    public void Invalidate()
    {
        if (IsExpired) return;
        IsExpired = true;
    }
}
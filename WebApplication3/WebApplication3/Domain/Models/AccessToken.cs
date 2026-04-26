namespace WebApplication3.Domain.Models;

public class AccessToken
{
    public Guid UserId { get; set; }
    public DateTime ExpirateAt { get; set; }

}
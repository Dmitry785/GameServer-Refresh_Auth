using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Domain.Models;
using WebApplication3.Infastructure;
using WebApplication3.Requests;
using WebApplication3.Responces;

namespace WebApplication3.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.AsNoTracking()
            .Where(x => x.Login == request.Login)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
            return Ok($"User with login {request.Login} not found"); // плохо!!!

        if (user.Password != request.Password)
            return Ok($"Invalid password"); // плохо!!!

        (var accessToken, var refreshToken) = GenerateTokens(user.Id);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var result = new LoginResult()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };

        return Ok(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var accessToken = JsonSerializer.Deserialize<AccessToken>(request.AccessToken);
        if (DateTime.UtcNow > accessToken.ExpirateAt)
        {
            return StatusCode(401);
        }
        Console.WriteLine($"User with Id: {accessToken.UserId}. Action: Logout");

        var refreshTokens = await dbContext.RefreshTokens
            .Where(x => x.UserId == accessToken.UserId)
            .Where(x => !x.IsExpired)
            .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Invalidate();
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var refreshToken = await dbContext.RefreshTokens
            .Where(x => x.Token == request.RefreshToken)
            .FirstOrDefaultAsync(cancellationToken);
        if (refreshToken == null)
            return StatusCode(401);

        if (DateTime.UtcNow > refreshToken.ExpirateAt)
        {
            refreshToken.Invalidate();
            await dbContext.SaveChangesAsync(cancellationToken);
            return StatusCode(401);
        }

        (var accessToken, var newRefreshToken) = GenerateTokens(refreshToken.UserId);
        dbContext.RefreshTokens.Add(newRefreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var result = new LoginResult()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };

        return Ok(result);
    }

    private (string accessToken, RefreshToken refreshToken) GenerateTokens(Guid userId)
    {
        var refreshToken = new RefreshToken(userId, DateTime.UtcNow + TimeSpan.FromHours(1));

        var accessToken = JsonSerializer.Serialize<AccessToken>(new AccessToken()
        {
            UserId = userId,
            ExpirateAt = DateTime.UtcNow + TimeSpan.FromMinutes(3)
        });

        return (accessToken, refreshToken);
    }
}

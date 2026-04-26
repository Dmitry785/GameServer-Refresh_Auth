using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Domain.Models;
using WebApplication3.Infastructure;
using WebApplication3.Requests;

namespace WebApplication3.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var users = await dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> GetById([FromBody] GetUserByIdRequest request, CancellationToken cancellationToken = default)
    {
        var accessToken = JsonSerializer.Deserialize<AccessToken>(request.AccessToken);
        if (DateTime.UtcNow > accessToken.ExpirateAt)
        {
            return StatusCode(401);
        }
        Console.WriteLine($"User with Id: {accessToken.UserId}. Action: GetById");

        var user = await dbContext.Users.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return Ok(user);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = new User(request.Login, request.Password);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(user.Id);
    }
}

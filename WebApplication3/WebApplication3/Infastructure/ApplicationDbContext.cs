using Microsoft.EntityFrameworkCore;
using WebApplication3.Domain.Models;

namespace WebApplication3.Infastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}

using Microsoft.EntityFrameworkCore;
using WebApplication3.Infastructure;

namespace WebApplication3;

public static class DependencyInjection
{
    public static IServiceCollection AddInfastructure(this IServiceCollection services)
    {
        string connectionString = $@"Server=(localdb)\MSSQLLocalDB;Database=GameCenter;Trusted_Connection=True;";
        services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));

        return services;
    }
}

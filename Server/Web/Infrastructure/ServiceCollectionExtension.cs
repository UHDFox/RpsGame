using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Web.Infrastructure;

public static class ServiceCollectionExtension
{
    public static void AddGameServerDbContext(this IServiceCollection services)
    {
        services.AddDbContext<GameServerDbContext>((provider, builder) =>
        {
            builder.UseNpgsql(provider.GetRequiredService<IConfiguration>().GetConnectionString("Psql"));
            builder.ConfigureWarnings(
                warnings => warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
        });
    }
}
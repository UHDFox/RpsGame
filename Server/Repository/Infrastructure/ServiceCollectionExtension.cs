using Microsoft.Extensions.DependencyInjection;
using Repository.GameTransactions;
using Repository.MatchHistory;
using Repository.User;

namespace Repository.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IMatchHistoryRepository, MatchHistoryRepository>();
        services.AddTransient<IGameTransactionsRepository, GameTransactionsRepository>();


        return services;
    }
}
using System.Reflection;
using Business.Game;
using Business.User;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddTransient<IGameManager, GameManager>();

        services.AddTransient<IUserService, UserService>();

        services.AddAutoMapper(ApplicationAssemblyReference.Assembly);


        return services;
    }

    private static class ApplicationAssemblyReference
    {
        public static Assembly Assembly => typeof(ApplicationAssemblyReference).Assembly;
    }
}
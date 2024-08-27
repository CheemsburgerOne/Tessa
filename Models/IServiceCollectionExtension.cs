using Tessa.Models.User;

namespace Tessa.Models;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddModelServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
namespace Tessa.Utilities.Configuration;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services )
    {
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        return services;
    }
    
}
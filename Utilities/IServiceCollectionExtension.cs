using Tessa.Services.DriveManager;
using Tessa.Utilities.Configuration;
using Tessa.Utilities.IconManager;
using Tessa.Utilities.User.UserFolderFactory;

namespace Tessa.Utilities;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddUtilities(this IServiceCollection services)
    {
        services.AddDriveManager();
        services.AddConfiguration();
        services.AddIconManager();
        services.AddUserFolderFactory();
        return services;
    }
    private static IServiceCollection AddDriveManager(this IServiceCollection services )
    {
        services.AddSingleton<IDriveManager, DriveManager.DriveManager>();
        return services;
    }
    private static IServiceCollection AddConfiguration(this IServiceCollection services )
    {
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        return services;
    }

    private static IServiceCollection AddIconManager(this IServiceCollection services)
    {
        services.AddSingleton<IIconManager, IconManager.IconManager>();
        return services;
    }

    private static IServiceCollection AddUserFolderFactory(this IServiceCollection services)
    {
        services.AddSingleton<UserFolderFactory, UserFolderFactory>();
        return services;
    }
}
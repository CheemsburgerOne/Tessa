using Tessa.Services.DriveManager;
using Tessa.Utilities.Configuration;

namespace Tessa.Utilities;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddDriveManager(this IServiceCollection services )
    {
        services.AddSingleton<IDriveManagerService, DriveManager.DriveManager>();
        return services;
    }
}
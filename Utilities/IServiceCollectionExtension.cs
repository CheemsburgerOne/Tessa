namespace Tessa.Utilities;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddDriveManager(this IServiceCollection services )
    {
        services.AddSingleton<IDriveManagerService, DriveManager>();
        return services;
    }
}
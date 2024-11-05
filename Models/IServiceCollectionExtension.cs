using Tessa.Models.Filesystem.Directory;
using Tessa.Models.Filesystem.File;
using Tessa.Models.User;

namespace Tessa.Models;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddModelServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDirectoryService, DirectoryService>();
        services.AddScoped<IFileService, FileService>();
        return services;
    }
}
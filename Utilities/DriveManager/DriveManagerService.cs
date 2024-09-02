using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.Extensions.Options;
using Tessa.Models.Filesystem;
using Tessa.Models.Filesystem.Directory;
using Tessa.Services.DriveManager;
using Tessa.Utilities.Configuration;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Tessa.Utilities.DriveManager;

public class DriveManager : IDriveManagerService
{
    private readonly string _root;

    public DriveManager(IConfigurationService configuration)
    {
        _root = configuration.Configuration.Root;
    }

    /// <summary>
    /// Create a file on the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including filename </param>
    /// <returns></returns>
    public FileStream? CreateFile(string path)
    {
        try
        {
            FileStream stream = File.Create($"{_root}{path}");
            return stream;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// Create a directory on the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including directory name </param>
    /// <returns></returns>
    public DirectoryInfo? CreateDirectory(string path)
    {
        try
        {
            DirectoryInfo dir = Directory.CreateDirectory($"{_root}{path}");
            return dir;
        }
        catch (Exception e)
        {
            return null;
        }

    }
}
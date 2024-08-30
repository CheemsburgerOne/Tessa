using Tessa.Models.Filesystem;
using Tessa.Models.Filesystem.Directory;
using Tessa.Services.DriveManager;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Tessa.Utilities.DriveManager;

public class DriveManager : IDriveManagerService
{
    private readonly DriveManagerConfiguration _configuration;

    public DriveManager(){}

    public void Configure(Action<DriveManagerConfiguration> configuration)
    {
        configuration(_configuration);
    }
/// <summary>
/// Create a file on the physical drive. <br/>
/// ex. "/root/path/to/file.txt"
/// </summary>
/// <param name="path">Relative path including filename </param>
/// <returns></returns>
    public async Task<FileStream?> CreateFileAsync(string path)
    {
        try
        {
            FileStream stream = File.Create($"{FilesystemConstants.Root}{path}" );
            return stream;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    /// <summary>
    /// Create a directory on the physical drive. <br/>
    /// ex. "/root/path/to/directory/"
    /// </summary>
    /// <param name="path">Relative path including directory name </param>
    /// <returns></returns>
    public DirectoryInfo? CreateDirectoryAsync(string path)
    {
        try
        {
            DirectoryInfo dir = Directory.CreateDirectory($"{FilesystemConstants.Root}{path}");
            return dir;
        }
        catch (Exception e)
        {
            return null;
        }

    }

    public bool Exists(string path)
    {
        return File.Exists(@"{this._configuration.Root}{path}");
    }
    
}
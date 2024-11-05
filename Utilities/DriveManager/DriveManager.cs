using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.Extensions.Options;
using Tessa.Models.Filesystem;
using Tessa.Models.Filesystem.Directory;
using Tessa.Services.DriveManager;
using Tessa.Utilities.Configuration;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Tessa.Utilities.DriveManager;
public class DriveManager : IDriveManager
{
    private readonly string _root;

    public DriveManager(IConfigurationService configuration)
    {
        _root = configuration.Configuration.Root;
    }

    #region FILES

    /// <summary>
    /// Create a file on the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including filename <br/>
    /// ex. /path/to/file.txt </param>
    /// <param name="buffer">Buffer for the filestream</param>
    /// <returns>Filestream for the file. Null if failed.</returns>
    public FileStream? CreateFile(string path, int buffer = 1024*1024)
    {
        try
        {
            FileInfo file = new FileInfo($"{_root}{path}");
            if (file.Exists) return null;
            FileStream stream = new FileStream(
                file.FullName, 
                FileMode.CreateNew, 
                FileAccess.Write, 
                FileShare.None,
                buffer,
                true
            );
            return stream;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    /// <summary>
    /// Moves a file to a new location on the physical drive. <br/>
    /// Acts as a rename if the path is the same. <br/>
    /// </summary>
    /// <param name="path">Path for the file</param>
    /// <param name="newName">New location</param>
    /// <returns>True if success, otherwise false</returns>
    public bool MoveFile(string path, string newName)
    {
        try
        {
            File.Move($"{_root}/{path}", $"{_root}/{newName}");
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    /// <summary>
    /// Deletes a file from the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including filename</param>
    /// <returns>True if success, otherwise false</returns>
    public bool DeleteFile(string path)
    {
        try
        {
            File.Delete($"{_root}/{path}");
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    #endregion

    #region DIRECTORIES
    
    /// <summary>
    /// Create a directory on the physical drive. <br/>
    /// </summary>
    /// <param name="path">
    /// Relative path including directory name <br/>
    /// ex. /path/to/folder/ </param>
    /// <returns>DirectoryInfo for the directory. Null if failed.</returns>
    public DirectoryInfo?  CreateDirectory(string path)
    {
        try
        {
            DirectoryInfo dir = Directory.CreateDirectory($"{_root}{path}/");
            return dir;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    /// <summary>
    /// Moves a directory to a new location on the physical drive. <br/>
    /// Acts as a rename if the path is the same. <br/>
    /// </summary>
    /// <param name="path">Path for the directory</param>
    /// <param name="newName">New location</param>
    /// <returns>True if success, otherwise false</returns>
    public bool MoveDirectory(string path, string newName)
    {
        try
        {
            Directory.Move($"{_root}/{path}", $"{_root}/{newName}");
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    /// <summary>
    /// Deletes a directory from the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including directory name</param>
    /// <returns>True if success, otherwise false</returns>
    public bool DeleteDirectory(string path)
    {
        try
        {
            Directory.Delete($"{_root}/{path}", true );
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    #endregion
    
}
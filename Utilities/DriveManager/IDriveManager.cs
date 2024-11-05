namespace Tessa.Services.DriveManager;

public interface IDriveManager
{
    /// <summary>
    /// Create a file on the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including filename <br/>
    /// ex. /path/to/file.txt </param>
    /// <param name="buffer">Buffer for the filestream</param>
    /// <returns>Filestream for the file. Null if failed.</returns>
    public FileStream? CreateFile(string path, int buffer = 1024 * 1024);

    /// <summary>
    /// Moves a file to a new location on the physical drive. <br/>
    /// Acts as a rename if the path is the same. <br/>
    /// </summary>
    /// <param name="path">Path for the file</param>
    /// <param name="newName">New location</param>
    /// <returns>True if success, otherwise false</returns>
    public bool MoveFile(string path, string newName);

    /// <summary>
    /// Deletes a file from the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including filename</param>
    /// <returns>True if success, otherwise false</returns>
    public bool DeleteFile(string path);
    
    /// <summary>
    /// Creates a directory at the specified path on a physical drive.
    /// </summary>
    /// <param name="path">Path at which a directory will be created</param>
    /// <returns>DirectoryInfo of the created directory, null upon failure</returns>
    public DirectoryInfo? CreateDirectory(string path);

    /// <summary>
    /// Moves a directory to a new location on the physical drive. <br/>
    /// Acts as a rename if the path is the same. <br/>
    /// </summary>
    /// <param name="path">Path for the directory</param>
    /// <param name="newName">New location</param>
    /// <returns>True if success, otherwise false</returns>
    public bool MoveDirectory(string path, string newName);

    /// <summary>
    /// Deletes a directory from the physical drive. <br/>
    /// </summary>
    /// <param name="path">Relative path including directory name</param>
    /// <returns>True if success, otherwise false</returns>
    public bool DeleteDirectory(string path);
}
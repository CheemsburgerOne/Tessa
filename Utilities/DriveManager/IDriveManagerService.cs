namespace Tessa.Services.DriveManager;

public interface IDriveManagerService
{
    /// <summary>
    /// Creates a files at the specified path on a physical drive.
    /// </summary>
    /// <param name="path">Path at which a directory will be created</param>
    /// <returns>FileStream of the created file, null upon failure</returns>
    public Task<FileStream?> CreateFileAsync(string path);
    /// <summary>
    /// Creates a directory at the specified path on a physical drive.
    /// </summary>
    /// <param name="path">Path at which a directory will be created</param>
    /// <returns>DirectoryInfo of the created directory, null upon failure</returns>
    public DirectoryInfo? CreateDirectoryAsync(string path);
}
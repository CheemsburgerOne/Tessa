using Tessa.Models.Filesystem.Directory;
using Tessa.Persistance.PostgreSQL;
using Tessa.Utilities.Configuration;
using Directory = System.IO.Directory;

namespace Tessa.Utilities.User.UserFolderFactory;

public class UserFolderFactory 
{
    private readonly string _userDataPath;
    private readonly string _userRootFolderPath;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public UserFolderFactory( IConfigurationService configurationService)
    {
        _userDataPath = configurationService.Configuration.Users;
        _userRootFolderPath = configurationService.Configuration.Root;
    }
    
    /// <summary>
    /// Creates a user specific folder. Containing: <br/>
    /// - "_icon.png"
    /// </summary>
    /// <param name="username">Username and thus folder name. Only valid chars.</param>
    /// <param name="icon">Filestream to the icon</param>
    /// <returns>True on success</returns>
    public async Task<bool> CreateUserDataFolder(string username, FileStream? icon = null)
    {
        string userPath = Path.Combine(_userDataPath, username);
        if ( Directory.Exists(userPath) ) return false;
        
        try
        {
            DirectoryInfo newDirectory = Directory.CreateDirectory(userPath);
            if (icon == null) return true;
            await using FileStream? emptyIcon = File.Create($"{newDirectory.FullName}/_icon.png");
            await icon.CopyToAsync(emptyIcon);
            await icon.DisposeAsync();
        }
        catch (Exception e)
        {
            Directory.Delete(userPath);
            return false;
        }
        
        return true;
    }
/// <summary>
/// Create user root folder containing uploaded files.
/// Path in database is "username/"
/// Does not insert the model into database. Must be done manually
/// </summary>
/// <param name="username">Caller username</param>
/// <param name="user">Caller id</param>
/// <returns>Directory model to insert</returns>
    public async Task<Models.Filesystem.Directory.Directory?> CreateUserRootFolder(string username, Guid user)
    {
        string rootPath = $"{_userRootFolderPath}{username}/";
        if ( Directory.Exists(rootPath) ) return null;
        
        try
        {
            DirectoryInfo newDirectory = Directory.CreateDirectory(rootPath);
        }
        catch (Exception e)
        {
            return null;
        }
        
        Models.Filesystem.Directory.Directory directory = new()
        {
            Name = $"{username}",
            OwnerId = user,
            Path = $"{username}/",
            Children = null,
            ParentId = null,
        };
        
        return directory;
    }
    
}
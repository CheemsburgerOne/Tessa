using Tessa.Models.Filesystem.Directory.ResultTypes;
using Tessa.Utilities.Result;

namespace Tessa.Models.Filesystem.Directory;
/// <summary>
/// Directory filesystem path methodology.
/// [root/] -> path/to/directory/ 
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Creates a directory on the physical drive and add an entry to database.
    /// </summary>
    /// <param name="dto">DTO containing all necessary information</param>
    /// <returns>GUID of the created entry. Null, if the operation failed</returns>
    public Task<Result<CreateDirectoryResultType>> CreateAsync(DirectoryEditDto dto);
    
    /// <summary>
    /// Gets the batched directory with all the information for HTTP reply.
    /// </summary>
    /// <param name="path">Database path of the directory</param>
    /// <returns>Object containing all the information. Null, if path is invalid or unauthorized access was detected</returns>
    public Task<Result<GetBatchedResultObject>> GetBatchedAsync(string path);
    
    /// <summary>
    /// Gets the GUID of the directory entry from the database.
    /// </summary>
    /// <param name="path">Database path of the object</param>
    /// <returns>Guid of the entry</returns>
    public Task<Guid?> GetGuidAsync(string path);
    public Task<bool> MoveAsync(string path, string newName);
    public Task<bool> RenameAsync(string path, string newName);
    public Task<Result<DeleteDirectoryResultType>> DeleteAsync(string path);
}
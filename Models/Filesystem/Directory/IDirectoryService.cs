namespace Tessa.Models.Filesystem.Directory;
/// <summary>
/// Directory filesystem path methodology.
/// [root/] -> path/to/directory/ 
/// </summary>
public interface IDirectoryService
{
    public Task<Guid?> CreateAsync(DirectoryEditDto dto);
    public Task<Directory?> GetAsync(string path);
}
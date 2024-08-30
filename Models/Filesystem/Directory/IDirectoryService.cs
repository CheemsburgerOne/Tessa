namespace Tessa.Models.Filesystem.Directory;

public interface IDirectoryService
{
    public Task<Guid?> CreateAsync(DirectoryEditDto dto);
    public Task<Directory?> GetAsync(string path);
}
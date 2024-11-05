using Tessa.Components;
using Tessa.Models.Filesystem.File;

namespace Tessa.Models.Filesystem.Directory;

public class DirectoryBatchedDto
{
    public string Path { get; set; }
    public string Name { get; set; }
    public string Owner { get; set; }
    public ushort ChildCount { get; set; }
    public List<DirectoryMiniDto> Directories { get; set; }
    public string DirectoryIconBase64 { get; set; }
    public List<FileMiniDto> Files { get; set; }
    
}
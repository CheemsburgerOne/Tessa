namespace Tessa.Models.Filesystem.File;

public class FileMiniDto
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    //TODO! Timestamp added last modified
    public string IconBase64 { get; set; }
}
using Microsoft.AspNetCore.Components.Forms;

namespace Tessa.Models.Filesystem.File;

public class UploadInputWrapper
{
    public string Path { get; set; }
    public string Name { get; set; } = "";
    public IReadOnlyList<IBrowserFile> Files { get; set; } = new List<IBrowserFile>();
}
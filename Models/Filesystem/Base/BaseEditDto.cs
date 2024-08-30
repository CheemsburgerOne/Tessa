namespace Tessa.Models.Filesystem.Base;

public abstract class BaseEditDto
{
    public Guid OwnerId { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
}
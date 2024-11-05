using System.ComponentModel.DataAnnotations;

namespace Tessa.Models.Filesystem.Base;

public abstract class Base
{
    public Guid Id { get; set; }
    
    public Guid? OwnerId { get; set; }
    
    public virtual User.User? Owner { get; set; }
    /// <summary>
    /// ID of the parent directory.
    /// </summary>
    public Guid? ParentId { get; set; }
    
    public virtual Directory.Directory? Parent { get; set; }
    
    [Required]
    [StringLength(40)]
    public string? Name { get; set; }
    
    /// <summary>
    /// Path to an item in the filesystem.<br/>
    /// Directories:
    /// [/root]/user/path/to/directory/ <br/>
    /// Files:
    /// [/root]/user/path/to/file.txt
    ///
    /// </summary>
    [Required]
    [StringLength(150)]
    public string? Path { get; set; }
    
    // public virtual IList<Event.Event>? Events { get; set; }
}
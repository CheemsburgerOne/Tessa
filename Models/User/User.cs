using System.ComponentModel.DataAnnotations;
using Tessa.Models.Filesystem.Base;

namespace Tessa.Models.User;

public class User
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(50)]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    
    [Required]
    [StringLength(20)]
    [DataType(DataType.Text)]
    public string? Username { get; set; }
    
    [Required]
    [StringLength(20)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    
    [Required]
    public UserType Type { get; set; }
    //
    // public virtual IList<Event.Event>? Events { get; set; }
    //
    public virtual IList<Base>? Items { get; set; }
}

public class UserEditDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }

    public UserType Type { get; set; }
    // public virtual IList<Event.Event>? Events { get; set; }
    // public virtual IList<Item>? Items { get; set; }
    public string? Password { get; set; }
}

public class LoginDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Tessa.Utilities.Configuration;

public class Configuration
{
    /// <summary>
    /// Default physical drive path for the drive manager. <br/>
    /// Points to the folder where all users' folders should be placed. <br/>
    /// Should be "Storage/root/"
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Root { get; set; }
    /// <summary>
    /// List of icons used for files. ex.<br/>
    /// [x][0] -> "rar" <br/>
    /// [x][1] -> "Resources/Icons/_rar.png"
    /// </summary>
    public string[][] Icons { get; set; }
    /// <summary>
    /// Folder where users are stored
    /// Should be "Storage/users/"
    /// </summary>
    public string Users { get; set; }
    /// <summary>
    /// Connection string to postgresql database
    /// </summary>
    public string PostgreSQLString { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
}

namespace Tessa.Utilities.PathHelper;

/// <summary>
/// Aggregates all directory path helper methods.
/// </summary>
public static class DirectoryPathHelper
{
    /// <summary>
    /// Normalizes path string by removing all consecutive slashes replacing them with a single slash.
    /// Also adds slashes at both end of the path.
    /// Used mainly to ensure that the directory path is in a correct format for database search. <br/>
    /// ex. "path///to//dir" -> "/path/to/dir/"
    /// </summary>
    /// <param name="path">Path to directory without the parent itelf</param>
    /// <returns>Normalized path</returns>
    public static string NormalizeDirectoryPath(this string path)
    {
        path = $"/{path}/";
        while (path.Contains("//"))
        {
            path = path.Replace("//", "/");
        }
        return path;
    }

    /// <summary>
    /// Normalizes directory name by removing all slashes.
    /// </summary>
    /// <param name="name">Directory name</param>
    /// <returns></returns>
    public static string NormalizeDirectoryName(this string name)
    {
        return name.Replace("/", "");
    }
    
    /// <summary>
    /// Checks if the directory path has valid characters.
    /// </summary>
    /// <param name="name">Directory path</param>
    /// <returns>True if name is correct, false otherwise</returns>
    public static bool PathHasValidChars(this string name)
    {
        return name.All( c => char.IsLetterOrDigit(c) || c == '/') ;
    }
    
    /// <summary>
    /// Checks if the directory name has valid characters.
    /// </summary>
    /// <param name="name">Directory name</param>
    /// <returns>True if name is correct, false otherwise</returns>
    public static bool NameHasValidChars(this string name)
    {
        return name.All( c => char.IsLetterOrDigit(c)) ;
    }
}
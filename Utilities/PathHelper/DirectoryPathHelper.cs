using System.Data;
using System.Text;

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
    /// ex. "path///to//dir" -> "username/path/to/dir/"
    /// </summary>
    /// <param name="path">Path to directory without the child itself</param>
    /// <param name="username">Username for normalization</param>
    /// <returns>Normalized path, returns "username/" on empty path</returns>
    public static string NormalizeDirectoryPath(this string path, string username)
    {
        // /  ->  username/
        if (path == "/" || path == "") return $"{username}/";
        
        byte[]? newPath = new byte[path.Length];
        uint newPathPosition = 0;
        bool nextConsecutiveSlash = true;
        int stop;
        
        foreach (char c in path)
        {
            if (char.IsLetterOrDigit(c))
            {
                newPath[newPathPosition] = (byte)c;
                newPathPosition += 1;
                nextConsecutiveSlash = false;
                continue;
            }
            
            if (nextConsecutiveSlash == true) continue;
            newPath[newPathPosition] = (byte)'/';
            newPathPosition += 1;
            nextConsecutiveSlash = true;
        }
        
        byte[] newPath2;
        if (newPath[newPathPosition-1] == '/')
        {
            newPath2 = new byte[newPathPosition];
        }
        else
        {
            newPathPosition += 1;
            newPath2 = new byte[newPathPosition];
            newPath2[newPathPosition-1] = (byte)'/';
        }
        for (int i = 0; i < newPathPosition-1; i++)
        {
            newPath2[i] = newPath[i];
        }
        
        string temp2 = Encoding.UTF8.GetString(newPath2);
        return $"{username}/{temp2}";;
        // return $"{username}/{new string(newPath)}";
    }

    // /// <summary>
    // /// Normalizes directory name by removing all slashes, then adds slash at the end
    // /// </summary>
    // /// <param name="name">Directory name</param>
    // /// <returns></returns>
    // public static string NormalizeDirectoryName(this string name)
    // {
    //     name = name.Replace("/", "");
    //     return $"{name}/";
    // }
    
    /// <summary>
    /// Checks if the directory path has valid characters. <br/>
    /// Permits only letters, digits and '/'.
    /// </summary>
    /// <param name="name">Directory path</param>
    /// <returns>True if name is correct, false otherwise</returns>
    public static bool PathHasValidChars(this string name)
    {
        return name.All( c => char.IsLetterOrDigit(c) || c == '/') ;
    }
    
    /// <summary>
    /// Checks if the directory name has valid characters. <br/>
    /// Permits only letters and digits.
    /// </summary>
    /// <param name="name">Directory name</param>
    /// <returns>True if name is correct, false otherwise</returns>
    public static bool NameHasValidChars(this string name)
    {
        return name.All( char.IsLetterOrDigit) ;
    }
    /// <summary>
    /// Extracts the parent path from the path.
    /// </summary>
    /// <param name="path">Path to extract from</param>
    /// <returns>Extracted parent path.</returns>
    /// <exception cref="ConstraintException">The path is already a root of the tree</exception>
    /// <exception cref="ArgumentException">Path is either empty or does not end with a '/'</exception>
    public static string GetDirectoryParentPath(this string path)
    {
        if(path == "/") throw new ConstraintException("Path does not have a parent. Probably a root folder");
        if (path == "" || path[^1] != '/') throw new ArgumentException("Path is invalid");
        
        int index = path[..^2].LastIndexOf('/');
        
        return index == -1 ? path : path[..(index+1)];
    }
}
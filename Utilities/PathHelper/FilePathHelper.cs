namespace Tessa.Utilities.PathHelper;

public static class FilePathHelper
{
    /// <summary>
    /// Normalizes path string by removing all consecutive slashes replacing them with a single slash.
    /// Also adds a slash at the beginning.
    /// Cuts off the last slash if it is the last character.
    /// Used mainly to ensure that the file path is in a correct format for database search.<br/>
    /// ex. "path///to//dir" -> "/path/to/dir"
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string NormalizeFilePath(this string path)
    {
        path = $"/{path}";
        while (path.Contains("//"))
        {
            path = path.Replace("//", "/");
        }
        if (path.EndsWith('/'))
        {
            path = path[..^1];
        }
        return path;
    }
    
    public static string NormalizeFileName(this string name)
    {
        return name.Replace("/", "");
    }
    
    /// <summary>
    /// Gets the extension of the file. <br/>
    /// ex. "file.txt" -> "txt"
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string? GetFileExtension(this string filename)
    {
        if (string.IsNullOrEmpty(filename)) return null;
        
        int index = filename.LastIndexOf('.');
        
        return index == -1 ? null : filename[(index + 1)..].ToLower();
    }
}
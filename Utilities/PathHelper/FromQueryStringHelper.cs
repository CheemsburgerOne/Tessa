namespace Tessa.Utilities.PathHelper;

public static class FromQueryStringHelper
{
    /// <summary>
    /// Replaces dashed path from http query to normal slash filesystem path. <br/>
    /// e.g. path-to-dir -> path/to/dir
    /// NormalizeDirectoryPath() should be called directly after this.
    /// </summary>
    /// <param name="path">Dashed path to transform</param>
    /// <returns>Path in slash format</returns>
    public static string FromDashedPath(this string path)
    {
        string newPath;
        newPath = path.Replace('-', '/');
        return newPath;
    }

}
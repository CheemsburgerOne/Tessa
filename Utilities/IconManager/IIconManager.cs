namespace Tessa.Utilities.IconManager;

public interface IIconManager
{
    public Icon GetIcon(string extension);
    public Dictionary<string, Icon> GetBatchedIcons(ISet<string> extensions, bool includeDir = true);
    public UserIcon GetUserIcon(string username);
    public bool InvalidateUserIcon(string username);
}
using Tessa.Utilities.Configuration;
using Tessa.Utilities.PathHelper;

namespace Tessa.Utilities.IconManager;

public class IconManager : IIconManager
{
    private readonly IConfigurationService _configurationService;
    private readonly Dictionary<string, Icon> _itemIcons = new();
    private readonly Dictionary<string, UserIcon> _volatileUserIcons = new();
    /// <summary>
    /// Folder where users are stored
    /// Should be "Storage/users/"
    /// </summary>
    private readonly string _usersPath;
    
    public IconManager(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
        _usersPath = configurationService.Configuration.Users;
        //Preload all item icons
        LoadItemIcons();
        //Preload default user icon
        LoadDefaultUserIcon();
    }
    
    /// <summary>
    /// Loads all icons from the configuration file into memory.
    /// If an icon failed to load it is skipped and will be replaced with default one.
    /// Dictionary ensures only one icon is corresponding to one extension.
    /// Although this function is not atomic, repeated calls will hot-swap the icons with new ones from the folder.
    /// </summary>
    private void LoadItemIcons()
    {
        // Load icons for the files.
        foreach (var icon in _configurationService.Configuration.Icons)
        {
            try
            {
                byte[] iconBytes = File.ReadAllBytes(icon[1]);
                string base64 = Convert.ToBase64String(iconBytes);
                Icon iconObject = new Icon
                {
                    Extension = icon[1].GetFileExtension()!,
                    Prefix = $"data:image/{icon[1].GetFileExtension()};base64,",
                    Base64 = base64,
                    FullBase64 = $"data:image/{icon[1].GetFileExtension()};base64,{base64}"
                };
                
                if (_itemIcons.TryAdd(icon[0], iconObject)) continue;
                _itemIcons.Remove(icon[0]);
                _itemIcons.Add(icon[0], iconObject);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        //Program fails if at least empty and dir icons are not loaded properly.
        if ( !( _itemIcons.TryGetValue("empty", out Icon _) && _itemIcons.TryGetValue("dir", out Icon _) ) ) Environment.Exit(1);
        
    }
    
    /// <summary>
    /// Gets the icon as a base64 string. <br/>
    /// If the extension has no associated icon, it returns the default one.
    /// </summary>
    /// <param name="extension">Extension associated with the icon.</param>
    /// <returns></returns>
    public Icon GetIcon(string extension)
    {
        if (_itemIcons.TryGetValue(extension, out Icon? fetched)) return fetched;
        _itemIcons.TryGetValue("empty", out Icon? substitution);
        return substitution!;

    }
    
    /// <summary>
    /// Gets batched icons in a single draw call. <br/>
    /// ISet interface ensures no duplicates are included.
    /// </summary>
    /// <param name="extensions">Extensions to include.</param>
    /// <param name="includeDir">Should directory icon be included.</param>
    /// <returns>Dictionary of [extension, icon] pairs</returns>
    public Dictionary<string, Icon> GetBatchedIcons(ISet<string> extensions, bool includeDir = true)
    {
        Dictionary<string, Icon> batch = new();
        
        if (includeDir)
        {
            _itemIcons.TryGetValue("dir", out Icon? dir);
            batch.Add("dir", dir!);
        }
        
        foreach (string item in extensions)
        {
            //If the icon is not found and default is not added yet.
            if (_itemIcons.TryGetValue(item, out Icon? fetched)) batch.Add(item, fetched);
            else 
            {
                _itemIcons.TryGetValue("empty", out Icon? placeholder);
                //TODO! All weird extensions shall share one placeholder icon to reduce batch size
                //Each "weird" extension has empty placeholder icon assigned to it.
                batch.Add(item, placeholder!);
            }
        }
        return batch;
    }

    /// <summary>
    /// Retrieves user icon from the volatile cache. <br/>
    /// If there is a cache miss it will try to load the icon from the disk.
    /// </summary>
    /// <param name="username">Username for request</param>
    /// <returns>Object containing data</returns>
    public UserIcon GetUserIcon(string username)
    {
        while (true)
        {
            //Retrieve icon
            if (_volatileUserIcons.TryGetValue(username, out UserIcon? fetchedIcon)) return fetchedIcon;
            
            //Unable to load from disc, iterate but as default user.
            if (!File.Exists($"{_usersPath}{username}/_icon.png"))
            {
                //Makes sure program won't enter endless default -> default loop
                if (username == "default")
                {
                    throw new FileNotFoundException("Critical error, default user icon might be corrupted");
                }
                username = "default";
                continue;
            }

            try
            {
                string base64 = Convert.ToBase64String(File.ReadAllBytes($"{_usersPath}{username}/_icon.png"));
                UserIcon userIcon = new UserIcon { Username = username, FullBase64 = $"data:image/png;base64,{base64}" };

                _volatileUserIcons.TryAdd(username, userIcon);

                return userIcon;
            }
            catch (Exception)
            {
                //Safety checks also here
                if (username == "default")
                {
                    throw new FileNotFoundException("Critical error, default user icon might be corrupted");
                }
                username = "default";
            }
        }
    }

    /// <summary>
    /// Loads default user icon into the volatile cache.
    /// Program fails if the icon is not loaded properly.
    /// </summary>
    private void LoadDefaultUserIcon()
    {
        try
        {
            string base64 = Convert
                .ToBase64String(
                    File.ReadAllBytes($"{_usersPath}default/_icon.png")
                );
            UserIcon userIcon = new UserIcon
            {
                Username = "default",
                FullBase64 = $"data:image/png;base64,{base64}"
            };

            _volatileUserIcons.TryAdd("default", userIcon);
        }
        catch (Exception)
        {
            throw new FileNotFoundException("Default user icon was not found", "/default/_icon_png");
        }
    }

    /// <summary>
    /// Invalidates user icon from the volatile cache by removing it.
    /// </summary>
    /// <param name="username">Username associated with the removed icon</param>
    /// <returns>True on success, otherwise false</returns>
    public bool InvalidateUserIcon(string username)
    { 
        return _volatileUserIcons.Remove(username);
    }
}
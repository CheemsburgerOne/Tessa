using Tessa.Utilities.Configuration;

namespace Tessa.Utilities.StartupManager;
/// <summary>
/// Manages all startup actions.
/// </summary>
public class StartupManager
{
    private readonly IConfigurationService _configurationService;
    public StartupManager(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }
    
    private void InitializeFolders()
    {
        //Initialize root folder
        if (
            string.IsNullOrEmpty(_configurationService.Configuration.PostgreSQLString) ||
            string.IsNullOrEmpty( _configurationService.Configuration.Root) ||
            string.IsNullOrEmpty( _configurationService.Configuration.Users) ||
            _configurationService.Configuration.Icons == null ) throw new NullReferenceException();
        
        InitializeOne(_configurationService.Configuration.Root);
        InitializeOne(_configurationService.Configuration.Users);
        
    }

    private void InitializeOne(string path)
    {
        if ( !Directory.Exists(path) )
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                throw new ArgumentException(path);
            }
        }
        
    }
    
}
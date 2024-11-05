using System.Text.Json;
namespace Tessa.Utilities.Configuration;
/// <summary>
/// Configuration service for the application. <br/>
/// All data is deserialized from ~/Configuration.json
/// </summary>
public class ConfigurationService : IConfigurationService
{
    public Configuration Configuration { get; set; } = new Configuration();
    public ConfigurationService()
    {
        Initialize();
    }
    /// <summary>
    /// Deserialize the configuration file into a Configuration object.
    /// </summary>
    private void Initialize()
    { 
        try
        {
            FileStream jsonStream = File.OpenRead("configuration.json");
            Configuration = JsonSerializer.Deserialize<Configuration>(jsonStream);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error loading configuration: " + e.Message);
            throw new FileNotFoundException("Configuration file is not found", "configuration.json");
            return;
        }
    }

}
using System.Text.Json;
namespace Tessa.Utilities.Configuration;
/// <summary>
/// Configuration service for the application. <br/>
/// All data is deserialized from ~/Configuration.json
/// </summary>
public class ConfigurationService : IConfigurationService
{
    public ConfigurationService()
    {
        Configuration = Initialize();
    }

    private Configuration Initialize()
    { 
        try
        {
            FileStream jsonStream = File.OpenRead("configuration.json");
            Configuration? config = JsonSerializer.Deserialize<Configuration>(jsonStream);
            return config!;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error loading configuration: " + e.Message);
            Environment.Exit(1);
            return null;
        }
    }
}
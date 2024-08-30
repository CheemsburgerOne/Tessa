namespace Tessa.Services.DriveManager;

public class DriveManagerConfiguration
{
    /// <summary>
    /// Default physical drive path for the drive manager. <br/>
    /// Points to the topmost folder where all data should be stored.
    /// </summary>
    public string Root { get; set; }
}
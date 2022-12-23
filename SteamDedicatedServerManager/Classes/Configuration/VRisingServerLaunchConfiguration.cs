namespace SteamDedicatedServerManager.Classes.Configuration;

public class VRisingServerLaunchConfiguration : IServerLaunchConfiguration
{
    /// <summary>
    /// Name of server in server list
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// Name of save file/directory
    /// </summary>
    public string SaveName { get; set; }

    /// <summary>
    /// Absolute or relative path to where Settings and Save files are held
    /// </summary>
    public string SaveFolderLocation { get; set; }
    
    /// <summary>
    /// Max number of concurrent players on server

    /// </summary>
    public long MaxConnectedUsers { get; set; }
    
    /// <summary>
    /// Max number of admins to allow connect even when server is full
    /// </summary>
    public long MaxConnectedAdmins { get; set; }
    
    /// <summary>
    /// Bind to a specific IP address; overrides machine address
    /// </summary>
    public string Address { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string LogPath { get; set; }
    
    /// <summary>
    /// UDP port for game traffic, TCP for rcon traffic
    /// </summary>
    public long GamePort { get; set; }
    
    /// <summary>
    /// UDP port for Steam server list features
    /// </summary>
    public long QueryPort { get; set; }
}
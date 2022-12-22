namespace VRisingServerManagement.Classes.Configuration;

public interface IServerLaunchConfiguration
{
    /// <summary>
    /// Name of server in server list
    /// </summary>
    string ServerName { get; set; }

    /// <summary>
    /// Name of save file/directory
    /// </summary>
    string SaveName { get; set; }

    /// <summary>
    /// Absolute or relative path to where Settings and Save files are held
    /// </summary>
    string SaveFolderLocation { get; set; }
    
    /// <summary>
    /// Max number of concurrent players on server

    /// </summary>
    long MaxConnectedUsers { get; set; }
    
    /// <summary>
    /// Max number of admins to allow connect even when server is full
    /// </summary>
    long MaxConnectedAdmins { get; set; }
    
    /// <summary>
    /// Bind to a specific IP address; overrides machine address
    /// </summary>
    string Address { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    string LogPath { get; set; }
    
    /// <summary>
    /// UDP port for game traffic, TCP for rcon traffic
    /// </summary>
    long GamePort { get; set; }
    
    /// <summary>
    /// UDP port for Steam server list features
    /// </summary>
    long QueryPort { get; set; }
}
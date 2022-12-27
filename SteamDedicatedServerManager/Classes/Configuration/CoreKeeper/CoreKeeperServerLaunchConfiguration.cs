namespace SteamDedicatedServerManager.Classes.Configuration.CoreKeeper;

public class CoreKeeperServerLaunchConfiguration : IServerLaunchConfiguration
{
    public string ServerName { get; set; }
    
    public string SaveName { get; set; }
    
    public string SaveFolderLocation { get; set; }
    
    public long MaxConnectedUsers { get; set; }
    
    public long MaxConnectedAdmins { get; set; }
    
    public string Address { get; set; }
    
    public string LogPath { get; set; }
    
    public long GamePort { get; set; }
    
    public long QueryPort { get; set; }
}
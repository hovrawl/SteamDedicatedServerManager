namespace SteamDedicatedServerManager.Classes.Configuration;

public interface IServerHostConfiguration
{
    string Name { get; set; }
    
    string Description { get; set; }
    
    long Port { get; set; }
    
    long QueryPort { get; set; }
    
    long MaxConnectedUsers { get; set; }
    
    long MaxConnectedAdmins { get; set; }
    
    string SaveName { get; set; }
    
    string Password { get; set; }
    
    bool ListOnMasterServer { get; set; }
    
    long AutoSaveCount { get; set; }
    
    long AutoSaveInterval { get; set; }
    
    string GameSettingsPreset { get; set; }

    RCONConfiguration RconConfiguration { get; set; }
}
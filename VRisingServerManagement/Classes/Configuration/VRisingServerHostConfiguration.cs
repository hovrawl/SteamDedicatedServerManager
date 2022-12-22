namespace VRisingServerManagement.Classes.Configuration;

public class VRisingServerHostConfiguration : IServerHostConfiguration
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public long Port { get; set; }
    
    public long QueryPort { get; set; }
    
    public long MaxConnectedUsers { get; set; }
    
    public long MaxConnectedAdmins { get; set; }
    
    public string SaveName { get; set; }
    
    public string Password { get; set; }
    
    public bool ListOnMasterServer { get; set; }
    
    public long AutoSaveCount { get; set; }
    
    public long AutoSaveInterval { get; set; }
    
    public string GameSettingsPreset { get; set; }

    public RCONConfiguration RconConfiguration { get; set; }

}
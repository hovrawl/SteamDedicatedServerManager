using SteamDedicatedServerManager.Classes.Server;
using SteamDedicatedServerManager.Enums;

namespace SteamDedicatedServerManager.Models;

public class ServerConfigViewModel
{
    public IServerInstance ServerInstance { get; set; }
    
    public ConfigType ConfigType { get; set; }
}
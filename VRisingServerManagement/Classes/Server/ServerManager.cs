using System.Text;
using SteamCMD.ConPTY;
using VRisingServerManagement.Classes.Configuration;
using VRisingServerManagement.Enums;

namespace VRisingServerManagement.Classes.Server;

public sealed class ServerManager
{
    // ----- Singleton Accessors ----- //

    private static ServerManager _instance = new();

    /// <summary>
    ///     Prevents a default instance of the <see cref="ServerManager" /> class from being created.
    /// </summary>
    private ServerManager()
    {
        
    }
    
    /// <summary>
    ///     Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ServerManager Instance => _instance;

    private static List<IServerInstance> _servers = new List<IServerInstance>();
    public static List<IServerInstance> Servers => _servers;

    private static string ServerPathInstallation = @".\Servers";

    public static IServerInstance CreateServer(GameServer gameServer)
    {
        IServerInstance returnInstance = null;
        switch (gameServer)
        {
            case GameServer.VRising:
            {
                var serverInstance = new VRisingServerInstance
                {
                    Id = Guid.NewGuid()
                };
                returnInstance = serverInstance;
                break;
            }
        }

        _servers.Add(returnInstance);
        return returnInstance;
    }

    public static IServerInstance? GetServer(Guid serverId)
    {
        return _servers.FirstOrDefault(i => i.Id.Equals(serverId));
    }
}
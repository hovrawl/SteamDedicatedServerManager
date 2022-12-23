using SteamCMD.ConPTY;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Enums;

namespace SteamDedicatedServerManager.Classes.Server;

public interface IServerInstance
{
    #region Fields
    Guid Id { get; set; }
    
    GameServer GameType { get; }
    
    WindowsPseudoConsole Console { get; set; }

    IServerLaunchConfiguration LaunchConfiguration { get; }
    
    IServerHostConfiguration HostConfiguration { get; }
    
    IServerGameConfiguration GameConfiguration { get; }
    #endregion
    
    #region Methods
    void StartServer();

    void SetLaunchConfiguration(IServerLaunchConfiguration launchConfiguration);
    
    void SetHostConfiguration(IServerHostConfiguration hostConfiguration);
    
    void SetGameConfiguration(IServerGameConfiguration gameConfiguration);
    #endregion
    
    #region Events
    void ServerTitleReceived(object? sender, string data);

    void ServerOutputDataReceived(object? sender, string data);

    void ServerExited(object? sender, int exitCode);
    
    #endregion
}
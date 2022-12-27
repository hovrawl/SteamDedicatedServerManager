using LiteDB;
using SteamCMD.ConPTY;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Services;

namespace SteamDedicatedServerManager.Classes.Server;

public interface IServerInstance
{
    #region Fields
    Guid Id { get; set; }
    
    string Name { get; set; }
    
    GameServer GameType { get; }
    
    ServerStatus ServerStatus { get; }
    
    [BsonIgnore]
    WindowsPseudoConsole Console { get; set; }

    IServerLaunchConfiguration LaunchConfiguration { get; }
    
    IServerHostConfiguration HostConfiguration { get; }
    
    IServerGameConfiguration GameConfiguration { get; }
    
    [BsonIgnore]
    IConsoleService ConsoleService { get; set; }

    #endregion
    
    #region Methods
    void StartServer();
    
    void StopServer();

    void SetLaunchConfiguration(IServerLaunchConfiguration launchConfiguration);
    
    void SetHostConfiguration(IServerHostConfiguration hostConfiguration);
    
    void SetGameConfiguration(IServerGameConfiguration gameConfiguration);
    
    void ConnectConsole();
    #endregion
    
    #region Events
    void ServerTitleReceived(object? sender, string data);

    void ServerOutputDataReceived(object? sender, string data);

    void ServerExited(object? sender, int exitCode);
    
    #endregion
}
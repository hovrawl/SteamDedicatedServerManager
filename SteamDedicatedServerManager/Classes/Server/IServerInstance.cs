using System.Diagnostics;
using LiteDB;
using Serilog;
using SteamCMD.ConPTY;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Services;
using ILogger = Serilog.ILogger;

namespace SteamDedicatedServerManager.Classes.Server;

public interface IServerInstance
{
    #region Fields
    Guid Id { get; set; }
    
    string Name { get; set; }
    
    GameServer GameType { get; }
    
    ServerStatus ServerStatus { get; }
    
    [BsonIgnore]
    Process ServerProcess { get; }

    IServerLaunchConfiguration LaunchConfiguration { get; }
    
    IServerHostConfiguration HostConfiguration { get; }
    
    IServerGameConfiguration GameConfiguration { get; }
    
    [BsonIgnore]
    IConsoleService ConsoleService { get; set; }

    [BsonIgnore] 
    ILogger Logger { get; }

    #endregion
    
    #region Methods

    void Init();
    
    void StartServer();
    
    void StopServer();

    void SetLaunchConfiguration(IServerLaunchConfiguration launchConfiguration);
    
    void SetHostConfiguration(IServerHostConfiguration hostConfiguration);
    
    void SetGameConfiguration(IServerGameConfiguration gameConfiguration);
    
    void ConnectConsole();
    #endregion
    
    #region Events

    void ServerErrorReceived(object sender, DataReceivedEventArgs e);

    void ServerMessageReceived(object sender, DataReceivedEventArgs e);

    void ServerExited(object sender, EventArgs e);
    
    #endregion
}
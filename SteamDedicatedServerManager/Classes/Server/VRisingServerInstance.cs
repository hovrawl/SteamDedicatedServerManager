using System.Diagnostics;
using System.Text;
using LiteDB;
using Serilog;
using SteamCMD.ConPTY;
using SteamCMD.ConPTY.Interop.Definitions;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Services;

namespace SteamDedicatedServerManager.Classes.Server;

public class VRisingServerInstance : IServerInstance
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public GameServer GameType => GameServer.VRising;
    
    public ServerStatus ServerStatus { get; private set; }

    [BsonIgnore]
    public WindowsPseudoConsole Console { get; set; }
    
    public IServerLaunchConfiguration LaunchConfiguration { get; set;  }
    public IServerHostConfiguration HostConfiguration { get; set;  }
    
    public IServerGameConfiguration GameConfiguration { get; set;  }

    [BsonIgnore]
    public IConsoleService ConsoleService { get; set; }
    
    public void StartServer()
    {
        if (Console == null)
        {
            CreateConsole();
        }

        Console.Start();
    }
    
    public void StopServer()
    {
        if (Console == null)
        {
            return;
        }

        Console.Dispose();
        
        Console = null;
    }


    public void SetLaunchConfiguration(IServerLaunchConfiguration launchConfiguration)
    {
        if (launchConfiguration is not VRisingServerLaunchConfiguration vRisingLaunchConfig) return;

        LaunchConfiguration = vRisingLaunchConfig;

        if (Console == null)
        {
            CreateConsole();
        }
    }

    private void CreateConsole()
    {
        var serverArguments = new StringBuilder();
        serverArguments.Append($"-persistentDataPath {LaunchConfiguration.SaveFolderLocation}");
        serverArguments.Append($"-serverName {LaunchConfiguration.ServerName}");
        serverArguments.Append($"-saveName {LaunchConfiguration.SaveName}");
        serverArguments.Append($"-logFile {LaunchConfiguration.LogPath}");
        
        var ServerExeName = @"VRisingServer.exe";
        var ServerPathInstallation = $@".\Servers\{GameType:G}";
        
        // Create server console process
        Console = new WindowsPseudoConsole
        {
            FileName = ServerExeName,
            WorkingDirectory = ServerPathInstallation,
            Arguments = string.Join(" ", serverArguments)
        };

        ConnectConsole();
    }

    public void ConnectConsole()
    {
        if (Console == null)
        {
            CreateConsole();
        }
        
        // Attach message handlers
        Console.TitleReceived += ServerTitleReceived;
        Console.OutputDataReceived += ServerOutputDataReceived;
        Console.Exited += ServerExited;
    }
    
    public void SetHostConfiguration(IServerHostConfiguration hostConfiguration)
    {
        if (hostConfiguration is not VRisingServerHostConfiguration vRisingServerHostConfiguration) return;

        HostConfiguration = vRisingServerHostConfiguration;
    }
    
    public void SetGameConfiguration(IServerGameConfiguration gameConfiguration)
    {
        if (gameConfiguration is not VRisingServerGameConfiguration vRisingGameConfig) return;

        GameConfiguration = vRisingGameConfig;
    }
    
    public void ServerTitleReceived(object? sender, string data)
    {
        Log.Information(data);
        ConsoleService?.SendMessage(data, false);
        ServerStatus = ServerStatus.Running;
    }
        
    public void ServerOutputDataReceived(object? sender, string data)
    {
        Log.Information(data);
        
        Task.Delay(550).Wait();

        ConsoleService?.SendMessage(data, false);
    }
    
    public void ServerExited(object? sender, int exitCode)
    {
        Log.Information("Server Closed");
        ConsoleService?.SendMessage("Server Closed");
        ServerStatus = ServerStatus.Stopped;

    }
}
using System.Text;
using Serilog;
using SteamCMD.ConPTY;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Enums;

namespace SteamDedicatedServerManager.Classes.Server;

public class VRisingServerInstance : IServerInstance
{
    public Guid Id { get; set; }
    
    public GameServer GameType => GameServer.VRising;

    public WindowsPseudoConsole Console { get; set; }
    
    public IServerLaunchConfiguration LaunchConfiguration { get; set;  }
    public IServerHostConfiguration HostConfiguration { get; set;  }
    
    public IServerGameConfiguration GameConfiguration { get; set;  }

    public void StartServer()
    {
        if (Console == null) return;

        Console.Start();
    }

    public void SetLaunchConfiguration(IServerLaunchConfiguration launchConfiguration)
    {
        if (launchConfiguration is not VRisingServerLaunchConfiguration vRisingLaunchConfig) return;

        LaunchConfiguration = vRisingLaunchConfig;
        
        var serverArguments = new StringBuilder();
        serverArguments.Append($"-persistentDataPath {LaunchConfiguration.SaveFolderLocation}");
        serverArguments.Append($"-serverName {LaunchConfiguration.ServerName}");
        serverArguments.Append($"-saveName {LaunchConfiguration.SaveName}");
        serverArguments.Append($"-logFile {LaunchConfiguration.LogPath}");
        
        var ServerExeName = @"VRisingServer.exe";
        var ServerPathInstallation = @".\Servers";
        if (Console == null)
        {
            // Create server console process

            Console = new WindowsPseudoConsole
            {
                FileName = ServerExeName,
                WorkingDirectory = ServerPathInstallation,
                Arguments = string.Join(" ", serverArguments)
            };
            // Attach message handlers

            Console.TitleReceived += ServerTitleReceived;
            Console.OutputDataReceived += ServerOutputDataReceived;
            Console.Exited += ServerExited;
        }
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
        //_consoleService.SendMessage(data, false);
    }
    
    public void ServerOutputDataReceived(object? sender, string data)
    {
        Log.Information(data);
        //_consoleService.SendMessage(data, false);
    }
    
    public void ServerExited(object? sender, int exitCode)
    {
        Log.Information("Server Closed");
        //_consoleService.SendMessage("Server Closed");
    }
}
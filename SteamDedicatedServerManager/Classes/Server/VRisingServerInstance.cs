using System.Diagnostics;
using System.Text;
using LiteDB;
using Serilog;
using SteamCMD.ConPTY;
using SteamCMD.ConPTY.Interop.Definitions;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Classes.Configuration.VRising;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Services;
using ILogger = Serilog.ILogger;

namespace SteamDedicatedServerManager.Classes.Server;

public class VRisingServerInstance : IServerInstance
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public GameServer GameType => GameServer.VRising;
    
    public ServerStatus ServerStatus { get; private set; }

    [BsonIgnore]
    public Process ServerProcess { get; private set; }

    public IServerLaunchConfiguration LaunchConfiguration { get; private set;  }
    public IServerHostConfiguration HostConfiguration { get; private set;  }
    
    public IServerGameConfiguration GameConfiguration { get; private set;  }

    [BsonIgnore]
    public IConsoleService ConsoleService { get; set; }
    
    [BsonIgnore] 
    public ILogger Logger { get; private set; }
    
    private string ServerExeName = @"VRisingServer.exe";
    private string ServerPathInstallation = $@"";
    
    public void Init()
    {
        ServerExeName = @"VRisingServer.exe";
        ServerPathInstallation = $@".\Servers\{GameType:G}";
        var logPath = @".\Logs";
        if (!string.IsNullOrEmpty(LaunchConfiguration?.LogPath))
        {
            logPath = LaunchConfiguration.LogPath;
        }
        Logger = new LoggerConfiguration().WriteTo.File(logPath).CreateLogger();
        
        CreateConsole();
    }
    
    public void StartServer()
    {
        if (ServerProcess == null)
        {
            CreateConsole();
        }

        ServerProcess.Start();
    }
    
    public void StopServer()
    {
        if (ServerProcess == null)
        {
            return;
        }

        ServerProcess.Kill();
        ServerProcess.Dispose();
        ServerProcess = null;
    }


    public void SetLaunchConfiguration(IServerLaunchConfiguration launchConfiguration)
    {
        if (launchConfiguration is not VRisingServerLaunchConfiguration vRisingLaunchConfig) return;

        LaunchConfiguration = vRisingLaunchConfig;

        UpdateLaunchArguments();
    }

    private void UpdateLaunchArguments()
    {
        if (ServerProcess == null)
        {
            CreateConsole();
        }
        else
        {
            var newArguments = GetLaunchArguments();
            ServerProcess.StartInfo.Arguments = newArguments;
        }
    }
    
    private string GetLaunchArguments()
    {
        if (LaunchConfiguration is not VRisingServerLaunchConfiguration launchConfig) return "";
        
        var launchArguments = new List<string>();
        
        launchArguments.Add($"-persistentDataPath {launchConfig.SaveFolderLocation}");
        launchArguments.Add($"-serverName {launchConfig.ServerName}");
        launchArguments.Add($"-saveName {launchConfig.SaveName}");
        
        var logPath = launchConfig.LogPath ?? $"{launchConfig.SaveFolderLocation}.txt";
        if (!logPath.EndsWith(".txt"))
        {
            logPath += ".txt";
        }

        logPath = Path.Combine(launchConfig.SaveFolderLocation, logPath);
        // Ensure log directory is available
    
        launchArguments.Add($@"-logfile {logPath}");
        
        return string.Join(" ", launchArguments);
    }
    
    private void CreateConsole()
    {
        var launchArguments = GetLaunchArguments();
        
        // Create server console process
        ServerProcess = new Process()
        {
            StartInfo =
            {
                WorkingDirectory = ServerPathInstallation,
                FileName = Path.Combine(ServerPathInstallation, ServerExeName),
                Arguments = launchArguments.TrimEnd(),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            },
            EnableRaisingEvents = true
        };

        ConnectConsole();
    }

    public void ConnectConsole()
    {
        if (ServerProcess == null)
        {
            CreateConsole();
        }
        
        // Attach message handlers
        ServerProcess.ErrorDataReceived += ServerErrorReceived;
        ServerProcess.OutputDataReceived += ServerMessageReceived;
        ServerProcess.Exited += ServerExited;
    }
    
    public void SetHostConfiguration(IServerHostConfiguration hostConfiguration)
    {
        if (hostConfiguration is not VRisingServerHostConfiguration vRisingServerHostConfiguration) return;

        HostConfiguration = vRisingServerHostConfiguration;
        // Save Host Config to JSON file
    }
    
    public void SetGameConfiguration(IServerGameConfiguration gameConfiguration)
    {
        if (gameConfiguration is not VRisingServerGameConfiguration vRisingGameConfig) return;

        GameConfiguration = vRisingGameConfig;
        // Save Game Config to JSON file

    }
    
    public void ServerTitleReceived(object? sender, string data)
    {
        Logger.Information(data);
        ConsoleService?.SendMessage(data, false);
        ServerStatus = ServerStatus.Running;
    }
    
    public void ServerErrorReceived(object sender, DataReceivedEventArgs e)
    {
        Logger.Error(e.Data);
        ConsoleService?.SendMessage(e.Data, true);
    }
    
    public void ServerMessageReceived(object sender, DataReceivedEventArgs e)
    {
        Logger.Information(e.Data);
        ConsoleService?.SendMessage(e.Data, false);
        ServerStatus = ServerStatus.Running;
    }
    
    public void ServerExited(object sender, EventArgs e)
    {
        Logger.Error("Server Closed");
        ConsoleService?.SendMessage("Server Closed");
        ServerStatus = ServerStatus.Stopped;
    }
}
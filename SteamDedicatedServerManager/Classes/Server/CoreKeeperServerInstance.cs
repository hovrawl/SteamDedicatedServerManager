using System.Diagnostics;
using System.Text;
using LiteDB;
using Serilog;
using SteamCMD.ConPTY;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Classes.Configuration.CoreKeeper;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Services;
using ILogger = Serilog.ILogger;

namespace SteamDedicatedServerManager.Classes.Server;

public class CoreKeeperServerInstance : IServerInstance
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public GameServer GameType => GameServer.CoreKeeper;

    public ServerStatus ServerStatus { get; private set; }
    
    public IServerLaunchConfiguration LaunchConfiguration { get; private set; }
    
    public IServerHostConfiguration HostConfiguration { get; private set; }
    
    public IServerGameConfiguration GameConfiguration { get; private set; }
    
    [BsonIgnore]
    public IConsoleService ConsoleService { get; set;}
    
    [BsonIgnore] 
    public ILogger Logger { get; private set; }

    private string ServerExeName = @"CoreKeeperServer.exe";
    private string WorkingDirectory = $@"";

    [BsonIgnore]
    public Process ServerProcess { get; private set; }
    
    public void Init()
    {
        ServerExeName = @"CoreKeeperServer.exe";
        WorkingDirectory = Environment.CurrentDirectory + $@"\Servers\{GameType:G}\";
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
        ServerStatus = ServerStatus.Running;

    }
    
    public void StopServer()
    {
        if (ServerProcess == null)
        {
            return;
        }

        ServerProcess.StandardInput.Close();
        ServerProcess.Kill();
        ServerProcess.Dispose();
        ServerProcess = null;
        ServerStatus = ServerStatus.Stopped;

    }


    public void SetLaunchConfiguration(IServerLaunchConfiguration launchConfiguration)
    {
        if (launchConfiguration is not CoreKeeperServerLaunchConfiguration vRisingLaunchConfig) return;

        LaunchConfiguration = vRisingLaunchConfig;

        UpdateLaunchArguments();
    }

    private void UpdateLaunchArguments()
    {
        //if (Console == null)
        if (ServerProcess == null)
        {
            CreateConsole();
        }
        else
        {
            var newArguments = GetLaunchArguments();
            //Console.Arguments = newArguments;
            ServerProcess.StartInfo.Arguments = newArguments;
        }
    }

    private string GetLaunchArguments()
    {
        if (LaunchConfiguration is not CoreKeeperServerLaunchConfiguration launchConfig) return "";

        // A GameID.txt file will be created next to the executable containing the Game ID.
        // If it doesn't appear you can check the log in the same location named CoreKeeperServerLog.txt for errors.
        
        // -world 0                                Which world index to use.
        // -logfile                                Log File, could include path
        // -worldname "Core Keeper Server"         The name to use for the server.
        // -worldseed 0                            The seed to use for a new world. Set to 0 to generate random seed.
        // -gameid ""                              Game ID to use for the server. Needs to be at least 28 characters and alphanumeric, excluding Y,y,x,0,O. Empty or not valid means a new ID will be generated at start.
        // -datapath ""                            Save file location. If not set it defaults to a subfolder named "DedicatedServer" at the default Core Keeper save location.
        // -maxplayers 100                         Maximum number of players that will be allowed to connect to server.
        // -worldmode 0                            Whether to use normal (0) or hard (1) mode for world.
        // -port <unset>                           What port to bind to. If not set, then the server will use the Steam relay network. If set the clients will connect to the server directly and the port needs to be open.
        // -ip 0.0.0.0                             Only used if port is set. Sets the address that the server will bind to.
        
        var launchArguments = new List<string>();
        
        launchArguments.Add($"-batchmode");

        var logDirectory = Path.Combine(WorkingDirectory, launchConfig.SaveFolderLocation);
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        
        var logPath = launchConfig.LogPath ?? $"{launchConfig.SaveFolderLocation}.txt";
        if (!logPath.EndsWith(".txt"))
        {
            logPath += ".txt";
        }

        logPath = Path.Combine(launchConfig.SaveFolderLocation, logPath);
        // Ensure log directory is available
    
        launchArguments.Add($@"-logfile {logPath}");
        
        launchArguments.Add($"-datapath {launchConfig.SaveFolderLocation}");

        
        if(!string.IsNullOrEmpty(launchConfig.GameId))
        launchArguments.Add($"-gameid {launchConfig.GameId}");

        launchArguments.Add($"-worldname {launchConfig.ServerName}");
            
        launchArguments.Add($"-port {launchConfig.GamePort}");
        
        launchArguments.Add($"-ip {launchConfig.Address}");


        return string.Join(" ", launchArguments);
    }
    
    private void CreateConsole()
    {
        var launchArguments = GetLaunchArguments();
        
        // Create server console process
        ServerProcess = new Process
        {
            StartInfo =
            {
                WorkingDirectory = WorkingDirectory,
                FileName = Path.Combine(WorkingDirectory, ServerExeName),
                Arguments = launchArguments.TrimEnd(),
                WindowStyle = ProcessWindowStyle.Minimized,
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
        ServerProcess.ErrorDataReceived += ServerErrorReceived;
        ServerProcess.OutputDataReceived += ServerMessageReceived;
        ServerProcess.Exited += ServerExited;
    }

  

    public void SetHostConfiguration(IServerHostConfiguration hostConfiguration)
    {
        if (hostConfiguration is not CoreKeeperServerHostConfiguration hostConfig) return;

        HostConfiguration = hostConfig;
    }
    
    public void SetGameConfiguration(IServerGameConfiguration gameConfiguration)
    {
        if (gameConfiguration is not CoreKeeperServerGameConfiguration gameConfig) return;

        GameConfiguration = gameConfig;
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
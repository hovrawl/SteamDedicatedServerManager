using System.Text;
using System.ComponentModel;
using Serilog;
using SteamCMD.ConPTY;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Classes.Extensions;
using SteamDedicatedServerManager.Services;

namespace SteamDedicatedServerManager.Classes.Server;

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

    #region Methods

    private static IConsoleService _consoleService;
    public static void SetConsoleMessages(IConsoleService consoleService)
    {
        _consoleService = consoleService;
    }
    
    /// <summary>
    /// Download game server if it does not exist, otherwise updates the files;
    /// </summary>
    /// <remarks>May cause issues if updating server files while servers for the same game are running</remarks>
    /// <param name="gameServer">Game Server</param>
    public static void DownloadServer(GameServer gameServer)
    {
        var steamCmdDirectory = @".\SteamCMD";
        if (!Directory.Exists(steamCmdDirectory))
        {
            Directory.CreateDirectory(steamCmdDirectory);
        }

        var login = "anonymous";
        var steamAppID = gameServer.GetDescription();
        var serverPathInstallation = $@"..\Servers\{gameServer:G}";
        var validateApp = ""; // set to 'validate' to validate the application
        var steamCMDConPTY = new SteamCMDConPTY
        {
            Arguments = $"+force_install_dir {serverPathInstallation} +login {login} +app_update {steamAppID} {validateApp} +quit"
        };
        var exited = false;
        var code = 0;
            
        steamCMDConPTY.WorkingDirectory = steamCmdDirectory;
        steamCMDConPTY.TitleReceived += (sender, data) =>
        {
            Log.Information(data);
            _consoleService?.SendMessage(data, false);
        };
        steamCMDConPTY.OutputDataReceived += (sender, data) =>
        {
            Log.Information(data);
            _consoleService?.SendMessage(data, false);
        };
        steamCMDConPTY.Exited += (sender, exitCode) =>
        {
            exited = true;
            code = exitCode;
            Log.Information("Server Downloaded");
            _consoleService?.SendMessage("Server Downloaded", false);        
        };
        steamCMDConPTY.Start();

        while (!exited)
        {
            Thread.Sleep(1000);   
        }

    }

    public static IServerInstance CreateServer(GameServer gameServer)
    {
        IServerInstance returnInstance = null;
        switch (gameServer)
        {
            case GameServer.VRising:
            {
                var serverInstance = new VRisingServerInstance
                {
                    Id = Guid.NewGuid(),
                    ConsoleService = _consoleService
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
    

    #endregion
   
}
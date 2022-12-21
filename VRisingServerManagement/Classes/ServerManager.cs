using System.Text;
using SteamCMD.ConPTY;

namespace VRisingServerManagement.Classes;

public sealed class ServerManager
{
    // ----- Singleton Accessors ----- //

    private static volatile ServerManager _instance;

    /// <summary>
    ///     The synchronize root
    /// </summary>
    private static readonly object SyncRoot = new object();
    
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
    public static ServerManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (SyncRoot)
            {
                if (_instance == null)
                    _instance = new ServerManager();
            }
            return _instance;
        }
    }

    private static ServerLaunchConfiguration LaunchConfiguration { get; set; } = new ();

    private static string  ServerExeName = @"VRisingServer.exe";
    private static string ServerPathInstallation = @".\Servers";

    public static WindowsPseudoConsole ServerConsole { get; private set; }
    
    public static void ConfigureLaunch(ServerLaunchConfiguration launchConfiguration)
    {
        LaunchConfiguration = launchConfiguration;
    }
    
    public static void ConfigureServerLaunch()
    {
        var serverArguments = new StringBuilder();
        serverArguments.Append($"-persistentDataPath {LaunchConfiguration.SaveFolderLocation}");
        serverArguments.Append($"-serverName {LaunchConfiguration.ServerName}");
        serverArguments.Append($"-saveName {LaunchConfiguration.SaveName}");
        serverArguments.Append($"-logFile {LaunchConfiguration.LogPath}");

        if (ServerConsole == null)
        {
            ServerConsole = new WindowsPseudoConsole
            {
                FileName = ServerExeName,
                WorkingDirectory = ServerPathInstallation,
                Arguments = string.Join(" ", serverArguments)
            };    
        }
    }

    public static bool StartServer()
    {
        if (ServerConsole == null) return false;

        ServerConsole.Start();

        return true;
    }
}
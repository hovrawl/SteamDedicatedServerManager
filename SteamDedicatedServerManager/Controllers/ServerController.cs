using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Classes.Configuration.CoreKeeper;
using SteamDedicatedServerManager.Classes.Configuration.VRising;
using SteamDedicatedServerManager.Classes.Server;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Models;
using SteamDedicatedServerManager.Services;

namespace SteamDedicatedServerManager.Controllers;

public class ServerController : Controller
{
    private readonly IConsoleService _consoleService;
    private readonly ILogger<HomeController> _logger;

    public ServerController(ILogger<HomeController> logger, IConsoleService consoleService)
    {
        _logger = logger;
        _consoleService = consoleService;
        
        // Setup serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        ServerManager.SetConsoleMessages(_consoleService);
    }
    // GET
    public IActionResult Index()
    {
        var servers = ServerManager.Servers;
   
        
        var viewModel = new ServerListViewModel
        {
            ServerInstances = servers
        };
        
        return View(viewModel);
    }
    
    public IActionResult Details(string serverIdString)
    {
        Guid.TryParse(serverIdString, out var serverId);
        var serverInstance = ServerManager.GetServer(serverId);
        if (serverInstance == null)
        {
            return RedirectToAction("Error", "Home");
        }
        
        var viewModel = new ServerDetailsViewModel
        {
            ServerInstance = serverInstance
        };
        
        return View(viewModel);
    }

    public JsonResult CreateServer(int gameServerInt, string serverName)
    {
        if (gameServerInt < 1 || string.IsNullOrEmpty(serverName))
        {
            return new JsonResult("incorrect values!")
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        
        var gameServer = (GameServer) gameServerInt;
        // check if the server files already exist
        ServerManager.DownloadServer(gameServer);

        IServerLaunchConfiguration launchConfig = null;
        switch (gameServer)
        {
            case GameServer.VRising:
            {
                launchConfig = new VRisingServerLaunchConfiguration
                {
                    ServerName = serverName,
                    SaveFolderLocation = @".\save-data",
                    SaveName = "Server Test",
                    LogPath = @".\logs\VRisingServer.log",
                };
                break;
            }

            case GameServer.Valheim:
            {
                
                break;
            }

            case GameServer.CoreKeeper:
            {
                launchConfig = new CoreKeeperServerLaunchConfiguration
                {
                    ServerName = serverName,
                    SaveFolderLocation = @".\save-data",
                    SaveName = "Server Test",
                    LogPath = @".\logs\CoreKeeper.log",
                };
                break;
            }
        }
        // Will take in game type arguments to create initial server instance
       
        
        var serverInstance = ServerManager.CreateServer(gameServer);
        
        serverInstance.SetLaunchConfiguration(launchConfig);
        serverInstance.Name = serverName;
        ServerManager.UpsertServer(serverInstance);
        
        return new JsonResult("server created!")
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    
    [HttpPost]
    public JsonResult StartServer(string serverIdString)
    {
        Guid.TryParse(serverIdString, out var serverId);
        var serverInstance = ServerManager.GetServer(serverId);
        if (serverInstance == null)
        {
            return new JsonResult("unable to find server")
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        serverInstance.StartServer();

        return new JsonResult("server started!")
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    
    [HttpPost]
    public JsonResult StopServer(string serverIdString)
    {
        Guid.TryParse(serverIdString, out var serverId);
        var serverInstance = ServerManager.GetServer(serverId);
        if (serverInstance == null)
        {
            return new JsonResult("unable to find server")
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        serverInstance.StopServer();

        return new JsonResult("server stopped!")
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    [HttpPost]
    public JsonResult ConnectConsole(string serverIdString)
    {
        Guid.TryParse(serverIdString, out var serverId);
        var serverInstance = ServerManager.GetServer(serverId);
        if (serverInstance == null)
        {
            return new JsonResult("unable to find server")
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        serverInstance.ConsoleService = _consoleService;

        serverInstance.ConnectConsole();
        
        return new JsonResult("Connected Console")
        {
            StatusCode = StatusCodes.Status200OK
        }; 
    }

    public JsonResult UpdateConfig(string serverIdString, int configTypeInt, string configString)
    {
        Guid.TryParse(serverIdString, out var serverId);
        var serverInstance = ServerManager.GetServer(serverId);
        if (serverInstance == null)
        {
            return new JsonResult("Failed to find server")
            {
                StatusCode = StatusCodes.Status400BadRequest
            }; 
        }

        var configType = (ConfigType)configTypeInt;
        switch (serverInstance.GameType)
        {
            case GameServer.CoreKeeper:
            {
                switch (configType)
                {
                    case ConfigType.Launch:
                    {
                        var launchConfig = new CoreKeeperServerLaunchConfiguration();
                        
                        var serverName = Request.Form["serverName"];
                        launchConfig.ServerName = serverName;
                        var saveFolder = Request.Form["saveFolder"];
                        launchConfig.SaveFolderLocation = saveFolder;
                        var saveName = Request.Form["saveName"];
                        launchConfig.SaveName = saveName;
                        long.TryParse(Request.Form["serverPort"], out var serverPort);
                        launchConfig.GamePort = serverPort;
                        var serverAddress = Request.Form["serverAddress"];
                        launchConfig.Address = serverAddress;
                        
                        serverInstance.SetLaunchConfiguration(launchConfig);
                        break;
                    }
                    case ConfigType.Host:
                    {
                        break;
                    }
                    case ConfigType.Game:
                    {
                        break;
                    }
                }
                break;
            }
            case GameServer.VRising:
            {
                switch (configType)
                {
                    case ConfigType.Launch:
                    {
                        break;
                    }
                    case ConfigType.Host:
                    {
                        break;
                    }
                    case ConfigType.Game:
                    {
                        break;
                    }
                }
                break;
            }
         
            case GameServer.Valheim:
            {
                switch (configType)
                {
                    case ConfigType.Launch:
                    {
                        break;
                    }
                    case ConfigType.Host:
                    {
                        break;
                    }
                    case ConfigType.Game:
                    {
                        break;
                    }
                }
                break;
            }
        }
        
        ServerManager.UpsertServer(serverInstance);
        
        return new JsonResult("Updated Config")
        {
            StatusCode = StatusCodes.Status200OK
        }; 
    }
    
    public PartialViewResult Console(string serverIdString)
    {
        Guid.TryParse(serverIdString, out var serverId);
        var serverInstance = ServerManager.GetServer(serverId);
        if (serverInstance == null)
        {
            return PartialView("Error"); 
        }
        var viewModel = new ServerDetailsViewModel
        {
            ServerInstance = serverInstance
        };
        return PartialView("_Console", viewModel); 
    }
}
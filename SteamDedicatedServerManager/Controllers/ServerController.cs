using Microsoft.AspNetCore.Mvc;
using Serilog;
using SteamDedicatedServerManager.Classes.Configuration;
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

    public JsonResult CreateServer()
    {
        var gameServer = GameServer.VRising;
        // check if the server files already exist
        ServerManager.DownloadServer(gameServer);

        // Will take in game type arguments to create initial server instance
        var launchConfig = new VRisingServerLaunchConfiguration
        {
            ServerName = "Breadland Test",
            SaveFolderLocation = @".\save-data",
            SaveName = "Server Test",
            LogPath = @".\logs\VRisingServer.log",
        };
        
        var serverInstance = ServerManager.CreateServer(gameServer);
        
        serverInstance.SetLaunchConfiguration(launchConfig);
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
}
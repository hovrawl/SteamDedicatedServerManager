using Microsoft.AspNetCore.Mvc;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Classes.Server;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Models;

namespace SteamDedicatedServerManager.Controllers;

public class ServerController : Controller
{
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
}
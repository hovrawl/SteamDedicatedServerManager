using Microsoft.AspNetCore.Mvc;
using SteamDedicatedServerManager.Classes.Server;
using SteamDedicatedServerManager.Models;

namespace SteamDedicatedServerManager.Controllers;

public class ServerController : Controller
{
    // GET
    public IActionResult Index(string serverIdString)
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
}
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SteamCMD.ConPTY;
using System;
using SteamDedicatedServerManager.Classes.Configuration;
using SteamDedicatedServerManager.Classes.Server;
using SteamDedicatedServerManager.Enums;
using SteamDedicatedServerManager.Models;
using SteamDedicatedServerManager.Services;
using SteamDedicatedServerManager.Classes;

namespace SteamDedicatedServerManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConsoleService _consoleService;
    private readonly StringBuilder _downloadConsoleMessageBuilder;

    public HomeController(ILogger<HomeController> logger, StringBuilder downloadConsoleMessageBuilder, IConsoleService consoleService)
    {
        _logger = logger;
        _downloadConsoleMessageBuilder = downloadConsoleMessageBuilder;
        _consoleService = consoleService;
        
        // Setup serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        ServerManager.SetConsoleMessages(_consoleService);
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Console()
    {
        return View();
    }
    
    public IActionResult Configuration()
    {
        return View();
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public JsonResult DownloadSteamCmd()
    {

        var steamCmdUri = new Uri("https://media.steampowered.com/installer/steamcmd.zip");
        var client = new HttpClient();
        var fileName = "steamcmd.zip";
        var directory = $"Downloads/";
        var filePath = Path.Combine(directory, fileName);
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        
        var response = client.GetAsync(steamCmdUri);

        using (var fs = new FileStream(filePath, FileMode.CreateNew))
        {
            response.Result.Content.CopyToAsync(fs);
        }
        
        var steamCmdDirectory = @".\SteamCMD";
        if (!Directory.Exists(steamCmdDirectory))
        {
            Directory.CreateDirectory(steamCmdDirectory);
        }
        else
        {
            Directory.Delete(steamCmdDirectory, true);
        }

        ZipFile.ExtractToDirectory(filePath, steamCmdDirectory);
        
        var returnResult = new JsonResult("ok");
        return returnResult;
    }

    // [HttpGet]
    // public async Task DownloadConsoleMessage()
    // {
    //     Response.ContentType = "text/event-stream";
    //
    //     if (_downloadConsoleMessageBuilder.Length > 0)
    //     {
    //         var messageBytes = Encoding.UTF8.GetBytes(_downloadConsoleMessageBuilder.ToString());
    //         await Response.Body.WriteAsync(messageBytes);
    //         _downloadConsoleMessageBuilder.Clear();
    //     }
    //    
    //     await Response.Body.FlushAsync();
    //
    // } 
    
    [HttpGet]
    public async Task DownloadConsoleMessage()
    {
        //Response.ContentType = "text/event-stream";
        await _consoleService.SendMessage("Test Message", false);
    } 
    
    // public ActionResult DownloadConsoleMessage()
    // {
    //     if (_downloadConsoleMessageBuilder.Length < 1) return Content("");
    //     
    //     var resultString = _downloadConsoleMessageBuilder.ToString();
    //     _downloadConsoleMessageBuilder.Clear();
    //     return Content(resultString, "text/event-stream");
    //
    // } 
   
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

  

 
}
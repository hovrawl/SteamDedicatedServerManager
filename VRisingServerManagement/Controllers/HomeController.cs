using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SteamCMD.ConPTY;
using VRisingServerManagement.Models;
using System;
using VRisingServerManagement.Services;

namespace VRisingServerManagement.Controllers;

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

    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Console()
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
    
    [HttpPost]
    public JsonResult DownloadVRisingServer()
    {
        var steamCmdDirectory = @".\SteamCMD";
        if (!Directory.Exists(steamCmdDirectory))
        {
            return new JsonResult("bad")
            {
                StatusCode = StatusCodes.Status204NoContent
            };
        }

        var login = "anonymous";
        var SteamAppID = "1829350";
        var ServerPathInstallation = @"..\Servers";
        var validateApp = ""; // set to 'validate' to validate the application
        var steamCMDConPTY = new SteamCMDConPTY()
        {
            Arguments = $"+force_install_dir {ServerPathInstallation} +login {login} +app_update {SteamAppID} {validateApp} +quit"
        };
        var exited = false;
        var code = 0;
            
        steamCMDConPTY.WorkingDirectory = steamCmdDirectory;
        steamCMDConPTY.TitleReceived += (sender, data) =>
        {
            Log.Information(data);
            _downloadConsoleMessageBuilder.Append(data);
            _consoleService.SendMessage(data, false);
        };
        steamCMDConPTY.OutputDataReceived += (sender, data) =>
        {
            Log.Information(data);
            _downloadConsoleMessageBuilder.Append(data);
            _consoleService.SendMessage(data, false);
        };
        steamCMDConPTY.Exited += (sender, exitCode) =>
        {
            exited = true;
            code = exitCode;
            //new HomeController(_logger).DownloadConsoleMessage("Server Downloaded");
            //DownloadConsoleMessage("Server Downloaded");
            _downloadConsoleMessageBuilder.Append("Server Downloaded");
        };
        steamCMDConPTY.Start();

        while (!exited)
        {
            Thread.Sleep(1000);   
        }

        // var steamCmdProcess = new Process();
        // steamCmdProcess.StartInfo.UseShellExecute = false;
        // steamCmdProcess.StartInfo.RedirectStandardError = true;
        // steamCmdProcess.StartInfo.RedirectStandardOutput = true;
        // steamCmdProcess.StartInfo.RedirectStandardInput = true;
        // steamCmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        // steamCmdProcess.StartInfo.CreateNoWindow = true;
        // steamCmdProcess.StartInfo.ErrorDialog = false;
        // steamCmdProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
        // steamCmdProcess.StartInfo.FileName = steamCmdExe;
        // steamCmdProcess.StartInfo.Arguments = $"+force_install_dir {ServerPathInstallation} +login {login} +app_update {SteamAppID} {validateApp} +quit";
        //
        // steamCmdProcess.Start();
        //
        // using (ManualResetEvent mreOut = new ManualResetEvent(false),
        //        mreErr = new ManualResetEvent(false))
        // {
        //     steamCmdProcess.OutputDataReceived += (o, e) =>
        //     {
        //         if (e.Data == null) mreOut.Set();
        //         else Log.Information(e.Data);
        //     };
        //     steamCmdProcess.BeginOutputReadLine();
        //     steamCmdProcess.ErrorDataReceived += (o, e) =>
        //     {
        //         if (e.Data == null) mreErr.Set();
        //         else Log.Error(e.Data);
        //     };
        //     steamCmdProcess.BeginErrorReadLine();
        //
        //     // var emptyLineCount = 0;
        //     // while (emptyLineCount < 3)
        //     // {
        //     //     steamCmdProcess.StandardInput.WriteLine("");
        //     //     emptyLineCount++;
        //     //     //var lineOutPut = steamCmdProcess.StandardOutput.ReadLine();
        //     //     // if (string.IsNullOrEmpty(lineOutPut))
        //     //     // {
        //     //     //     emptyLineCount++;
        //     //     // } 
        //     //     // else
        //     //     // {
        //     //     //     Log.Information(lineOutPut);
        //     //     //     emptyLineCount = 0;
        //     //     // }
        //     // }
        //     
        //     //
        //     // var output = steamCmdProcess.StandardOutput.ReadLine();
        //     // while (!ModalChecker.IsWaitingForUserInput(steamCmdProcess))
        //     // {
        //     //     steamCmdProcess.StandardInput.WriteLine("ww");
        //     //     Thread.Sleep(1000);   
        //     // }
        //     //var result = steamCmdProcess.StandardOutput.ReadToEnd();
        //     
        //     steamCmdProcess.WaitForExit();
        //
        //     steamCmdProcess.StandardInput.Close();
        //     mreOut.WaitOne();
        //     mreErr.WaitOne();
        // }
        // var code = steamCmdProcess.ExitCode;
        // //steamCmdProcess.WaitForExit();
        // steamCmdProcess.Dispose();
        var returnResult = new JsonResult(code)
        {
            StatusCode = StatusCodes.Status200OK
        };
        
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
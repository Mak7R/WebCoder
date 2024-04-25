using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCoder.WebUI.ViewModels;

namespace WebCoder.WebUI.Controllers;

[AllowAnonymous]
[Controller]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
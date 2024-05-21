using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebCoder.WebUI.Controllers;

[Controller]
[AllowAnonymous]
[Route("/help")]
public class HelpController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("repository-commands")]
    public IActionResult RepositoryCommands()
    {
        return View();
    }
}
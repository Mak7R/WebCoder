using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.WebUI.Extensions;
using WebCoder.WebUI.ViewModels;
using FileResult = WebCoder.Application.Models.FileResult;

// ReSharper disable ConvertToPrimaryConstructor

namespace WebCoder.WebUI.Controllers;


// DANGER CONTROLLER // HAS NO CHECK OF AUTHORIZATION OF USER // EVERY USER HAS ACCESS TO EVERY REPOSITORY DATA

[Controller]
[Route("/repositories/{userName}/{title}/sources")]
public class RepositorySourcesController : Controller
{
    private readonly IProjectRepositoriesService _repositoriesService ;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepositorySources _repositorySources;
    private readonly IRepositoryCommandHandler _repositoryCommandHandler;

    public RepositorySourcesController(IProjectRepositoriesService repositoriesService,
        UserManager<ApplicationUser> userManager, IRepositorySources repositorySources,
        IRepositoryCommandHandler repositoryCommandHandler)
    {
        _repositoriesService = repositoriesService;
        _userManager = userManager;
        _repositorySources = repositorySources;
        _repositoryCommandHandler = repositoryCommandHandler;
    }

    // api 
    [HttpGet("structure")]
    [Authorize]
    public async Task<IActionResult> GetStructure(string userName, string title)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var structure = await _repositorySources.GetStructure(user, userName, title);
        return Json(structure);
    }
    
    // view
    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> Index(string userName, string title)
    {
        var repository = await _repositoriesService.GetRepositoryByOwnerAndTitle(userName, title);
        
        if (repository is null) return this.NotFoundView("Repository was not found");
        
        // check rights
        
        return View();
    }

    // command handler
    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> ExecuteCommand([FromRoute] string userName, [FromRoute] string title,
        [FromForm] CommandDto commandDto)
    {
        if (string.IsNullOrEmpty(commandDto.Command))
        {
            ModelState.AddModelError(nameof(commandDto.Command), "Command was empty");
            return View("Index", commandDto);
        }      
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        var result = await _repositoryCommandHandler.Execute(user, userName, title, commandDto.Command, commandDto.Sources);
        try
        {
            if (!result.IsSuccessful)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(nameof(commandDto.Command), error);
            
                return View("Index", commandDto);
            }

            if (result.ResultObject is FileResult fileResult)
            {
                return File(fileResult.FileStream, System.Net.Mime.MediaTypeNames.Application.Octet,
                    fileResult.FileName);
            }
        }
        catch
        {
            switch (result.ResultObject)
            {
                case Stream fileStream:
                    await fileStream.DisposeAsync();
                    break;
                case FileResult fileResult:
                    await fileResult.FileStream.DisposeAsync();
                    break;
            }
        }
        
        return RedirectToAction("Index", new {userName, title});
    }

    [Authorize]
    [HttpGet("get-file")]
    public async Task<IActionResult> GetFile([FromRoute] string userName, [FromRoute] string title, [FromQuery] string? filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var result = await _repositoryCommandHandler.Execute(user, userName, title, $"LOAD FILE \"{filePath}\"");

        if (!result.IsSuccessful)
        {
            return BadRequest(result.Errors);
        }
        
        if (result.ResultObject is FileResult fileResult)
        {
            return File(fileResult.FileStream, System.Net.Mime.MediaTypeNames.Text.Plain,
                fileResult.FileName);
        }

        return NotFound((IEnumerable<string>)["File was not found"]);
    }

    [Authorize]
    [HttpPost("update-file")]
    public async Task<IActionResult> UpdateFile([FromRoute] string userName, [FromRoute] string title, [FromForm] UpdateFileDto updateFileDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        using var memoryStream = new MemoryStream(Encoding.Default.GetBytes(updateFileDto.FileData));
        
        var userPath = updateFileDto.FilePath;
        userPath = userPath.Replace('/', Path.DirectorySeparatorChar);
        userPath = userPath.StartsWith(Path.DirectorySeparatorChar) ? userPath.Substring(1) : userPath;
        userPath = userPath.StartsWith(title + Path.DirectorySeparatorChar) ? userPath.Substring(title.Length + 1) : userPath;
        
        await _repositorySources.UpdateFile(user, userName, title, userPath, memoryStream);

        return RedirectToAction("Index", new {userName, title});
    }
}
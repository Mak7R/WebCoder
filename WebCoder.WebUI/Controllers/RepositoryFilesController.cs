using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.WebUI.Extensions;
// ReSharper disable ConvertToPrimaryConstructor

namespace WebCoder.WebUI.Controllers;


// DANGER CONTROLLER // HAS NO CHECK OF AUTHORIZATION OF USER // EVERY USER HAS ACCESS TO EVERY REPOSITORY DATA

[Controller]
[Route("/repositories/{userName}/{title}/sources")]
public class RepositoryFilesController : Controller
{
    private readonly IProjectRepositoriesService _repositoriesService ;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepositoryFilesService _repositoryFilesService;
    private readonly IRepositoryCommandHandler _repositoryCommandHandler;

    public RepositoryFilesController(IProjectRepositoriesService repositoriesService,
        UserManager<ApplicationUser> userManager, IRepositoryFilesService repositoryFilesService,
        IRepositoryCommandHandler repositoryCommandHandler)
    {
        _repositoriesService = repositoriesService;
        _userManager = userManager;
        _repositoryFilesService = repositoryFilesService;
        _repositoryCommandHandler = repositoryCommandHandler;
    }

    // api 
    [HttpGet("structure")]
    public async Task<IActionResult> GetStructure(string userName, string title)
    {
        return Json(_repositoryFilesService.GetStructure(userName, title));
    }
    
    // view
    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> EditSources(string userName, string title)
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
            return View("EditSources", commandDto);
        }      
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        var result = _repositoryCommandHandler.Execute(userName, title, commandDto.Command, commandDto.Sources);
        if (!result.IsSuccessful)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(nameof(commandDto.Command), error);
            
            return View("EditSources", commandDto);
        }
        
        return View("EditSources");
    }
}
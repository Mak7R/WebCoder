using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.WebUI.Enums;
using WebCoder.WebUI.Extensions;
using WebCoder.WebUI.ViewModels.Repository;

namespace WebCoder.WebUI.Controllers;

[Controller]
[Route("/repositories")]
public class RepositoriesController(IProjectRepositoriesService repositoriesService, UserManager<ApplicationUser> userManager) : Controller
{
    private readonly IProjectRepositoriesService _repositoriesService = repositoriesService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> UserRepositories()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account", new {ReturnUrl = "/repositories/add"});

        var repositories = await _repositoriesService.GetUserRepositories(user);
        return View(repositories.Select(r => new RepositoryViewModel{Id = r.Id, Title = r.Title, IsPublic = r.IsPublic, Owner = r.OwnerUserName, About = r.About}));
    }
    
    [AllowAnonymous]
    [HttpGet("{userName}/{title}")]
    public async Task<IActionResult> GetRepository(string userName, string title)
    {
        var repository = await _repositoriesService.GetRepositoryByOwnerAndTitle(userName, title);

        if (repository is null) return this.NotFoundView("Repository was not found \ud83d\ude1e");
        
        var permission = UserPermission.ViewOnly;
        if (User.Identity?.Name == userName) permission = UserPermission.OwnerPermission;
        
        return View(new RepositoryViewModel
        {
            Id = repository.Id, 
            Title = repository.Title, 
            Owner = repository.OwnerUserName, 
            About = repository.About, 
            IsPublic = repository.IsPublic,
            UserPermission = permission
        });
    }
    
    [Authorize]
    [HttpGet("add")]
    public IActionResult AddRepository()
    {
        return View();
    }
    
    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddRepository(AddRepositoryDto addRepositoryDto)
    {
        if (!ModelState.IsValid)
        {
            return View(addRepositoryDto);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        await _repositoriesService.AddRepository(addRepositoryDto, user);

        return RedirectToAction("GetRepository", new { userName = user.UserName, title = addRepositoryDto.Title });
    }


    [Authorize]
    [HttpGet("{userName}/{title}/edit")]
    public async Task<IActionResult> EditRepository(string userName, string title)
    {
        var repository = await _repositoriesService.GetRepositoryByOwnerAndTitle(userName, title);
        
        if (repository is null) return this.NotFoundView("Repository was not found");
        
        var updateRepository = new UpdateRepositoryDto
        {
            Title = repository.Title,
            About = repository.About,
            IsPublic = repository.IsPublic
        };
        
        return View(updateRepository);
    }

    [Authorize]
    [HttpPost("{userName}/{title}/edit")]
    public async Task<IActionResult> EditRepository([FromRoute]string userName, [FromRoute] string title, [FromForm] UpdateRepositoryDto updateRepositoryDto)
    {
        if (!ModelState.IsValid) return View(updateRepositoryDto);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) if (user == null) return RedirectToAction("Login", "Account");
        
        await _repositoriesService.UpdateRepository(userName, title, updateRepositoryDto, user);
        return RedirectToAction("GetRepository", new { userName = user.UserName, title = updateRepositoryDto.Title });
    }
    
    [Authorize]
    [Route("{userName}/{title}/delete")]
    public async Task<IActionResult> DeleteRepository(string userName, string title)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) if (user == null) return RedirectToAction("Login", "Account");
        
        await _repositoriesService.RemoveRepository(userName, title, user);
        return RedirectToAction("Index", "Home");
    }
}
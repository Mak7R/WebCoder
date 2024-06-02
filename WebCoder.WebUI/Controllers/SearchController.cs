using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCoder.Application.Interfaces;
using WebCoder.WebUI.ViewModels.Repository;

namespace WebCoder.WebUI.Controllers;

[AllowAnonymous]
[Controller]
[Route("[controller]")]
public class SearchController : Controller
{
    private readonly IProjectRepositoriesService _repositoriesService;

    public SearchController(IProjectRepositoriesService repositoriesService)
    {
        _repositoriesService = repositoriesService;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string? title)
    {
        var repositories = await _repositoriesService.GetRepositories();
        if (!string.IsNullOrWhiteSpace(title)) repositories = repositories.Where(r => r.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase));

        ViewBag.SearchTitle = title ?? string.Empty;
        
        return View(repositories.Select(r => new RepositoryViewModel{Id = r.Id, Title = r.Title, IsPublic = r.IsPublic, Owner = r.OwnerUserName, About = r.About}));
    }
}
using Microsoft.Extensions.Logging;
using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.Application.RepositoryInterfaces;
using WebCoder.Domain.Models;

namespace WebCoder.Application.Services;

public class ProjectRepositoriesService : IProjectRepositoriesService
{
    private readonly IProjectRepositoriesRepository _projectRepositoriesRepository;
    private readonly ILogger<ProjectRepositoriesService> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ProjectRepositoriesService(
        IProjectRepositoriesRepository projectRepositoriesRepository, ILogger<ProjectRepositoriesService> logger)
    {
        _projectRepositoriesRepository = projectRepositoriesRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RepositoryDto>> GetRepositories()
    {
        var repositories = await _projectRepositoriesRepository.GetRepositories();
        var projectRepositories = repositories as List<ProjectRepository> ?? repositories.ToList();
        _logger.LogInformation("Received {Count} repositories from IProjectRepositoryService", projectRepositories.Count);
        return projectRepositories.Select(pr => new RepositoryDto
        {
            Id = pr.Id,
            Title = pr.Title,
            OwnerUserName = pr.OwnerUserName,
            IsPublic = pr.IsPublic,
            About = pr.About,
            CreationDate = pr.CreationDate
        });
    }

    public async Task<IEnumerable<RepositoryDto>> GetUserRepositories(ApplicationUser user)
    {
        var repositories = await _projectRepositoriesRepository.GetRepositoriesForUser(user.UserName ?? string.Empty);
        var projectRepositories = repositories as List<ProjectRepository> ?? repositories.ToList();
        _logger.LogInformation("Received {Count} repositories from IProjectRepositoryService for user ({User})", projectRepositories.Count, user.UserName);
        return projectRepositories.Select(pr => new RepositoryDto
        {
            Id = pr.Id,
            Title = pr.Title,
            OwnerUserName = pr.OwnerUserName,
            IsPublic = pr.IsPublic,
            About = pr.About,
            CreationDate = pr.CreationDate
        });
    }

    public async Task<RepositoryDto?> GetRepositoryById(Guid repositoryId)
    {
        var repository = await _projectRepositoriesRepository.GetProjectRepositoryById(repositoryId);
        _logger.LogInformation("Repository with id ({Id}) was {Not} found", repositoryId, repository == null ? "not" : string.Empty);
        if (repository is null) return null;

        return new RepositoryDto
        {
            Id = repository.Id,
            Title = repository.Title,
            OwnerUserName = repository.OwnerUserName,
            IsPublic = repository.IsPublic,
            About = repository.About,
            CreationDate = repository.CreationDate
        };
    }

    public async Task<RepositoryDto?> GetRepositoryByOwnerAndTitle(string userName, string title)
    {
        var repository = await _projectRepositoriesRepository.GetProjectRepositoryByOwnerAndTitle(userName, title);
        _logger.LogInformation("Repository with owner and title ({Repository}) was {Not} found", $"{userName}/{title}", repository == null ? "not" : string.Empty);
        if (repository is null) return null;

        return new RepositoryDto
        {
            Id = repository.Id,
            Title = repository.Title,
            OwnerUserName = repository.OwnerUserName,
            IsPublic = repository.IsPublic,
            About = repository.About,
            CreationDate = repository.CreationDate
        };
    }
}
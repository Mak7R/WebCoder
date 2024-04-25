using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.Application.RepositoryInterfaces;
using WebCoder.Domain.Exceptions;
using WebCoder.Domain.Models;

namespace WebCoder.Application.Services;

public class ProjectRepositoriesService : IProjectRepositoriesService
{
    private readonly IProjectRepositoriesRepository _projectRepositoriesRepository;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public ProjectRepositoriesService(IProjectRepositoriesRepository projectRepositoriesRepository)
    {
        _projectRepositoriesRepository = projectRepositoriesRepository;
    }
    
    public async Task AddRepository(AddRepositoryDto addRepositoryDto, ApplicationUser user)
    {
        var now = DateTime.Now;
        var repositoryId = Guid.NewGuid();
        
        var repository = new ProjectRepository
        {
            Id = repositoryId,
            Title = addRepositoryDto.Title,
            OwnerUserName = user.UserName ?? string.Empty,
            About = addRepositoryDto.About,
            IsPublic = addRepositoryDto.IsPublic,
            CreationDate = now
        };

        await _projectRepositoriesRepository.AddRepository(repository);
    }

    public async Task<IEnumerable<RepositoryDto>> GetRepositories()
    {
        var repositories = await _projectRepositoriesRepository.GetRepositories();

        return repositories.Select(pr => new RepositoryDto
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

        return repositories.Select(pr => new RepositoryDto
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
    
    public async Task UpdateRepository(string userName, string title, UpdateRepositoryDto updateRepositoryDto, ApplicationUser user)
    {
        var repository = await _projectRepositoriesRepository.GetProjectRepositoryByOwnerAndTitle(userName, title);
        if (repository == null) throw new NotFoundException("Repository was not found");

        if (repository.OwnerUserName != user.UserName) throw new PermissionDeniedException("User is not an owner of this repository");

        await _projectRepositoriesRepository.UpdateRepository(repository.Id, pr =>
        {
            pr.Title = updateRepositoryDto.Title;
            pr.About = updateRepositoryDto.About;
            pr.IsPublic = updateRepositoryDto.IsPublic;
        });
    }

    public async Task RemoveRepository(string userName, string title, ApplicationUser user)
    {
        var repository = await _projectRepositoriesRepository.GetProjectRepositoryByOwnerAndTitle(userName, title);
        if (repository == null) throw new NotFoundException("Repository was not found");

        if (repository.OwnerUserName != user.UserName) throw new PermissionDeniedException("User is not an owner of this repository");
        
        await _projectRepositoriesRepository.RemoveRepository(repository.Id);
    }
}
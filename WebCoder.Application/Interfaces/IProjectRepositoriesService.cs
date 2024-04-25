using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Identity;

namespace WebCoder.Application.Interfaces;

public interface IProjectRepositoriesService
{
    Task AddRepository(AddRepositoryDto addRepositoryDto, ApplicationUser user);

    Task<IEnumerable<RepositoryDto>> GetRepositories();
    Task<IEnumerable<RepositoryDto>> GetUserRepositories(ApplicationUser user);

    Task<RepositoryDto?> GetRepositoryById(Guid repositoryId);
    Task<RepositoryDto?> GetRepositoryByOwnerAndTitle(string userName, string title);

    Task UpdateRepository(string userName, string title, UpdateRepositoryDto updateRepositoryDto, ApplicationUser user);

    Task RemoveRepository(string userName, string title, ApplicationUser user);
}
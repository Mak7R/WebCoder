using WebCoder.Domain.Models;

namespace WebCoder.Application.RepositoryInterfaces;

public interface IProjectRepositoriesRepository
{
    Task AddRepository(ProjectRepository repository);
    
    Task<IEnumerable<ProjectRepository>> GetRepositoriesForUser(string userName);
    Task<IEnumerable<ProjectRepository>> GetRepositories(int takeCount = -1, int skipCount = 0, bool publicOnly = true);
    Task<ProjectRepository?> GetProjectRepositoryById(Guid repositoryId);
    Task<ProjectRepository?> GetProjectRepositoryByOwnerAndTitle(string userName, string title);
    
    Task UpdateRepository(Guid repositoryId, Action<ProjectRepository> updater);
    
    Task RemoveRepository(Guid repositoryId);
}
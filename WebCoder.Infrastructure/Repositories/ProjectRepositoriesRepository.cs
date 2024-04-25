using Microsoft.EntityFrameworkCore;
using WebCoder.Application.RepositoryInterfaces;
using WebCoder.Domain.Exceptions;
using WebCoder.Domain.Models;
using WebCoder.Infrastructure.Data;

namespace WebCoder.Infrastructure.Repositories;

public class ProjectRepositoriesRepository(ApplicationDbContext dbContext) : IProjectRepositoriesRepository
{
    public async Task AddRepository(ProjectRepository repository)
    {
        await dbContext.ProjectRepositories.AddAsync(repository);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProjectRepository>> GetRepositoriesForUser(string userName)
    {
        return await dbContext.ProjectRepositories
            .AsNoTracking()
            .Where(pr => pr.OwnerUserName == userName)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectRepository>> GetRepositories(int takeCount = -1, int skipCount = 0, bool publicOnly = true)
    {
        var query = dbContext.ProjectRepositories.AsNoTracking();
        
        if (publicOnly) query = query.Where(pr => pr.IsPublic);
        
        query = query.Skip(skipCount);
        
        if (takeCount >= 0) query = query.Take(takeCount);

        return await query.ToListAsync();
    }

    public async Task<ProjectRepository?> GetProjectRepositoryById(Guid repositoryId)
    {
        return await dbContext.ProjectRepositories.AsNoTracking().FirstOrDefaultAsync(pr => pr.Id == repositoryId);
    }

    public async Task<ProjectRepository?> GetProjectRepositoryByOwnerAndTitle(string userName, string title)
    {
        return await dbContext.ProjectRepositories
            .AsNoTracking()
            .FirstOrDefaultAsync(pr => pr.OwnerUserName == userName && pr.Title == title);
    }

    public async Task UpdateRepository(Guid repositoryId, Action<ProjectRepository> updater)
    {
        var repository = await dbContext.ProjectRepositories
            .FirstOrDefaultAsync(pr => pr.Id == repositoryId);

        if (repository == null) throw new NotFoundException($"Repository with id <{repositoryId}> was not found");

        updater(repository);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveRepository(Guid repositoryId)
    {
        var repository = await dbContext.ProjectRepositories
            .FirstOrDefaultAsync(pr => pr.Id == repositoryId);

        if (repository == null) throw new NotFoundException($"Repository with id <{repositoryId}> was not found");

        dbContext.Remove(repository);
        await dbContext.SaveChangesAsync();
    }
}
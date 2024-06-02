using Microsoft.EntityFrameworkCore;
using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.RepositoryInterfaces;
using WebCoder.Domain.Exceptions;
using WebCoder.Domain.Models;
using WebCoder.Infrastructure.Data;
using WebCoder.Infrastructure.Entities;

namespace WebCoder.Infrastructure.Repositories;

public class ProjectRepositoriesRepository(ApplicationDbContext dbContext) : IProjectRepositoriesRepository
{
    public async Task AddRepository(ProjectRepository repository)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == repository.OwnerUserName) ?? throw new NotFoundException("User was not found");
        var repositoryEntity = new ProjectRepositoryEntity
        {
            Id = repository.Id,
            Owner = user,
            Title = repository.Title,
            About = repository.About,
            IsPublic = repository.IsPublic,
            CreationDate = repository.CreationDate
        };
        
        await dbContext.ProjectRepositories.AddAsync(repositoryEntity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProjectRepository>> GetRepositoriesForUser(string? userName = null)
    {
        return await dbContext.ProjectRepositories
            .AsNoTracking()
            .Where(pr => pr.Owner.UserName == userName)
            .Select(entity => new ProjectRepository
            {
                Id = entity.Id, 
                Title = entity.Title, 
                About = entity.About, 
                OwnerUserName = entity.Owner.UserName ?? string.Empty,
                CreationDate = entity.CreationDate,
                IsPublic = entity.IsPublic
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectRepository>> GetRepositories(int takeCount = -1, int skipCount = 0, bool publicOnly = true)
    {
        var query = dbContext.ProjectRepositories.AsNoTracking();
        
        if (publicOnly) query = query.Where(pr => pr.IsPublic);
        
        query = query.Skip(skipCount);
        
        if (takeCount >= 0) query = query.Take(takeCount);

        return await query.Select(entity => new ProjectRepository
        {
            Id = entity.Id, 
            Title = entity.Title, 
            About = entity.About, 
            OwnerUserName = entity.Owner.UserName ?? string.Empty,
            CreationDate = entity.CreationDate,
            IsPublic = entity.IsPublic
        }).ToListAsync();
    }

    public async Task<ProjectRepository?> GetProjectRepositoryById(Guid repositoryId)
    {
        return await dbContext.ProjectRepositories.AsNoTracking().Select(entity => new ProjectRepository
        {
            Id = entity.Id, 
            Title = entity.Title, 
            About = entity.About, 
            OwnerUserName = entity.Owner.UserName ?? string.Empty,
            CreationDate = entity.CreationDate,
            IsPublic = entity.IsPublic
        }).FirstOrDefaultAsync(pr => pr.Id == repositoryId);
    }

    public async Task<ProjectRepository?> GetProjectRepositoryByOwnerAndTitle(string userName, string title)
    {
        return await dbContext.ProjectRepositories
            .AsNoTracking()
            .Select(entity => new ProjectRepository
            {
                Id = entity.Id, 
                Title = entity.Title, 
                About = entity.About, 
                OwnerUserName = entity.Owner.UserName ?? string.Empty,
                CreationDate = entity.CreationDate,
                IsPublic = entity.IsPublic
            })
            .FirstOrDefaultAsync(pr => pr.OwnerUserName == userName && pr.Title == title);
    }

    public async Task UpdateRepository(Guid repositoryId, UpdateRepositoryDto updateRepositoryDto)
    {
        var repository = await dbContext.ProjectRepositories
            .FirstOrDefaultAsync(pr => pr.Id == repositoryId);

        if (repository == null) throw new NotFoundException($"Repository with id <{repositoryId}> was not found");

        repository.Title = updateRepositoryDto.Title;
        repository.About = updateRepositoryDto.About;
        repository.IsPublic = updateRepositoryDto.IsPublic;
        
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
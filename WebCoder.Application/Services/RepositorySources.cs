using Microsoft.AspNetCore.Http;
using RepositoriesStorage.RepositoriesRepository;
using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Extensions;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.Application.RepositoryInterfaces;
using WebCoder.Domain.Exceptions;
using WebCoder.Domain.Models;

namespace WebCoder.Application.Services;

public class RepositorySources : IRepositorySources
{
    private const string AllowedTitleCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
    private class UserPermissions
    {
        private enum UserRight
        {
            NoRights,
            View,
            Update,
            Owner,
            Admin
        }

        private readonly UserRight _right;
        private UserPermissions(UserRight right)
        {
            _right = right;
        }

        public static UserPermissions Admin => new UserPermissions(UserRight.Admin);
        public static UserPermissions Owner => new UserPermissions(UserRight.Owner);
        public static UserPermissions Update => new UserPermissions(UserRight.Update);
        public static UserPermissions View => new UserPermissions(UserRight.View);
        public static UserPermissions NoRights => new UserPermissions(UserRight.NoRights);

        public bool HasAdminRights => _right switch
        {
            UserRight.Admin => true,
            _ => false
        };
    
        public bool HasOwnerRights => _right switch
        {
            UserRight.Owner => true,
            _ => false
        };
    
        public bool HasUpdateRights => _right switch
        {
            UserRight.Admin or UserRight.Owner or UserRight.Update => true,
            _ => false
        };
    
        public bool HasViewRights => _right switch
        {
            UserRight.Admin or UserRight.Owner or UserRight.Update or UserRight.View => true,
            _ => false
        };
    }
    private async Task<(UserPermissions, ProjectRepository)> GetUserPermissions(ApplicationUser user, string userName, string repositoryTitle)
    {
        var repository = await _projectRepositoriesRepository.GetProjectRepositoryByOwnerAndTitle(userName, repositoryTitle);
        if (repository == null) throw new NotFoundException("Repository was not found");
        
        if (user.UserName == userName) return (UserPermissions.Owner, repository);
        // if (user is Admin) return UserPermissions.Admin;
        // if (user is Contributor) return UserPermissions.Update;
        return (repository.IsPublic ? UserPermissions.View : UserPermissions.NoRights, repository);
    }
    
    private readonly IRepositoriesRepository _repositoriesRepository;
    private readonly IProjectRepositoriesRepository _projectRepositoriesRepository;

    public RepositorySources(IRepositoriesRepository repositoriesRepository, IProjectRepositoriesRepository projectRepositoriesRepository)
    {
        _repositoriesRepository = repositoriesRepository;
        _projectRepositoriesRepository = projectRepositoriesRepository;
    }

    public async Task CreateRepository(ApplicationUser user, CreateRepositoryDto createRepositoryDto)
    {
        var now = DateTime.Now;
        var repositoryId = Guid.NewGuid();

        if (!createRepositoryDto.Title.ContainsOnlyCharacters(AllowedTitleCharacters))
            throw new ArgumentException("Title must contain only latin characters, digits or -");
        
        var repository = new ProjectRepository
        {
            Id = repositoryId,
            Title = createRepositoryDto.Title,
            OwnerUserName = user.UserName ?? string.Empty,
            About = createRepositoryDto.About,
            IsPublic = createRepositoryDto.IsPublic,
            CreationDate = now
        };

        try
        {
            await _projectRepositoriesRepository.AddRepository(repository);
            await _repositoriesRepository.CreateRepository(new DefaultRepositoryIdentity(user.UserName!, createRepositoryDto.Title));
        }
        catch (Exception ex) // TODO EXCEPTIONS SYSTEM
        {
            throw new ArgumentException($"You already had repository with title '{createRepositoryDto.Title}'", ex);
        }
    }
    
    public async Task UpdateRepository(ApplicationUser user, string userName, string repositoryTitle, UpdateRepositoryDto updateRepositoryDto)
    {
        var (permission, repository) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");
        
        if (!updateRepositoryDto.Title.ContainsOnlyCharacters(AllowedTitleCharacters))
            throw new ArgumentException("Title must contain only latin characters, digits or -");

        try
        {
            await _projectRepositoriesRepository.UpdateRepository(repository.Id, updateRepositoryDto);

            if (repositoryTitle != updateRepositoryDto.Title)
            {
                await _repositoriesRepository.UpdateRepository(
                    new DefaultRepositoryIdentity(user.UserName!, repositoryTitle),
                    new DefaultRepositoryIdentity(user.UserName!, updateRepositoryDto.Title));
            }
        }
        catch (Exception ex) // TODO EXCEPTIONS SYSTEM
        {
            throw new ArgumentException($"You already had repository with title '{updateRepositoryDto.Title}'", ex);
        }
    }

    public async Task DeleteRepository(ApplicationUser user, string repositoryTitle)
    {
        var (permission, repository) = await GetUserPermissions(user, user.UserName!, repositoryTitle);
        if (!permission.HasOwnerRights)
            throw new PermissionDeniedException("User has not owner permission");
        
        await _projectRepositoriesRepository.RemoveRepository(repository.Id);
        await _repositoriesRepository.DeleteRepository(new DefaultRepositoryIdentity(user.UserName!, repositoryTitle));
    }
    
    public async Task<FileSystemTreeNode> GetStructure(ApplicationUser user, string userName, string repositoryTitle)
    {
        var (permission, _) = await GetUserPermissions(user, user.UserName!, repositoryTitle);
        if (!permission.HasViewRights)
            throw new NotFoundException("Repository was not found");
        
        return _repositoriesRepository.GetStructure(new DefaultRepositoryIdentity(userName, repositoryTitle));
    }

    public async Task AddDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.AddDirectory(new DefaultRepositoryIdentity(userName, repositoryTitle), dirFullName);
    }

    public async Task AddFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.AddFile(new DefaultRepositoryIdentity(userName, repositoryTitle), fileFullName);
    }

    public async Task DeleteDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.DeleteDirectory(new DefaultRepositoryIdentity(userName, repositoryTitle), dirFullName);
    }

    public async Task DeleteFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.DeleteFile(new DefaultRepositoryIdentity(userName, repositoryTitle), fileFullName);
    }

    public async Task CopyDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName,
        string dirDestFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.CopyDirectory(new DefaultRepositoryIdentity(userName, repositoryTitle), dirFullName, dirDestFullName);
    }

    public async Task CopyFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName,
        string fileDestFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.CopyFile(new DefaultRepositoryIdentity(userName, repositoryTitle), fileFullName, fileDestFullName);
    }

    public async Task MoveDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName,
        string dirDestFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.MoveDirectory(new DefaultRepositoryIdentity(userName, repositoryTitle), dirFullName, dirDestFullName);
    }

    public async Task MoveFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName,
        string fileDestFullName)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await _repositoriesRepository.MoveFile(new DefaultRepositoryIdentity(userName, repositoryTitle), fileFullName, fileDestFullName);
    }

    public async Task InsertDirectory(ApplicationUser user, string userName, string repositoryTitle, string parentDirPath,
        IFormFile archive)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await using var archiveStream = archive.OpenReadStream();
        await _repositoriesRepository.InsertDirectory(new DefaultRepositoryIdentity(userName, repositoryTitle), parentDirPath, archiveStream);
    }

    public async Task InsertFile(ApplicationUser user, string userName, string repositoryTitle, string parentDirPath, IFormFile file)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await using var fileStream = file.OpenReadStream();
        await _repositoriesRepository.InsertFile(new DefaultRepositoryIdentity(userName, repositoryTitle), Path.Combine(parentDirPath, file.FileName), fileStream);
    }

    public async Task<Stream> GetDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullPath)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");
        
        return await _repositoriesRepository.GetDirectory(new DefaultRepositoryIdentity(userName, repositoryTitle), dirFullPath);
    }

    public async Task<Stream> GetFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullPath)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");
        
        return await _repositoriesRepository.GetFile(new DefaultRepositoryIdentity(userName, repositoryTitle), fileFullPath);
    }

    public async Task UpdateFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullPath, IFormFile file)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");

        await using var fileStream = file.OpenReadStream();
        await _repositoriesRepository.UpdateFile(new DefaultRepositoryIdentity(userName, repositoryTitle), fileFullPath, fileStream);
    }
    
    public async Task UpdateFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullPath, Stream fileStream)
    {
        var (permission, _) = await GetUserPermissions(user, userName, repositoryTitle);
        if (!permission.HasUpdateRights)
            throw new PermissionDeniedException("User has not update permission");
        
        await _repositoriesRepository.UpdateFile(new DefaultRepositoryIdentity(userName, repositoryTitle), fileFullPath, fileStream);
    }
}
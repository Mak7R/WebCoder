using Microsoft.AspNetCore.Http;
using RepositoryStoragesRealizations.DirectoryStorage.RepositoriesStorage;
using WebCoder.Application.Interfaces;

namespace WebCoder.Application.Services;

public class RepositoryFilesService : IRepositoryFilesService
{
    private readonly IDirectoryRepositoriesStorage _storage;

    public RepositoryFilesService(IDirectoryRepositoriesStorage storage)
    {
        _storage = storage;
    }

    public Task CreateRepository(string userName, string repositoryTitle)
    {
        _storage.CreateRepository(userName, repositoryTitle);
        return Task.CompletedTask;
    }

    public Task DeleteUserRepositories(string userName)
    {
        _storage.DeleteUser(userName);
        return Task.CompletedTask;
    }

    public Task DeleteRepository(string userName, string repositoryTitle)
    {
        _storage.DeleteRepository(userName, repositoryTitle);
        return Task.CompletedTask;
    }

    public Task<FileSystemTreeNode> GetStructure(string userName, string repositoryTitle)
    {
        return Task.FromResult(_storage.GetStructure(userName, repositoryTitle));
    }
    

    public Task CreateDirectory(string userName, string repositoryTitle, string dirFullName)
    {
        _storage.AddDirectory(userName, repositoryTitle, dirFullName);
        return Task.CompletedTask;
    }
    
    public Task DeleteDirectory(string userName, string repositoryTitle, string dirFullName)
    {
        _storage.DeleteDirectory(userName, repositoryTitle, dirFullName);
        return Task.CompletedTask;
    }
    
    public Task MoveDirectory(string userName, string repositoryTitle, string dirFullName, string dirNewFullName)
    {
        _storage.MoveDirectory(userName, repositoryTitle, dirFullName, dirNewFullName);
        return Task.CompletedTask;
    }

    public async Task InsertDirectory(string userName, string repositoryTitle, string parentDirPath, IFormFile file)
    {
        var directoryInfo = _storage.TempStorage.Allocate();
        var archivePath = Path.Combine(directoryInfo.FullPath, file.FileName);
        await using (var fileStream = new FileStream(archivePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        _storage.InsertDirectory(userName, repositoryTitle, parentDirPath, archivePath);
        _storage.TempStorage.Deallocate(directoryInfo.Id);
    }
}
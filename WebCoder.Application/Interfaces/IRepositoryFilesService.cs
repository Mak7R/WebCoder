using Microsoft.AspNetCore.Http;
using RepositoryStoragesRealizations.DirectoryStorage.RepositoriesStorage;
using WebCoder.Application.Identity;

namespace WebCoder.Application.Interfaces;

public interface IRepositoryFilesService
{
    Task CreateRepository(string userName, string repositoryTitle);
    Task DeleteUserRepositories(string userName);
    Task DeleteRepository(string userName, string repositoryTitle);
    Task<FileSystemTreeNode> GetStructure(string userName, string repositoryTitle);
    Task CreateDirectory(string userName, string repositoryTitle, string dirFullName);
    Task DeleteDirectory(string userName, string repositoryTitle, string dirFullName);
    Task MoveDirectory(string userName, string repositoryTitle, string dirFullName, string dirNewFullName);
    Task InsertDirectory(string userName, string repositoryTitle, string parentDirPath, IFormFile file);
}
using Microsoft.AspNetCore.Http;
using RepositoriesStorage.RepositoriesRepository;
using WebCoder.Application.DTOs.ProjectRepository;
using WebCoder.Application.Identity;

namespace WebCoder.Application.Interfaces;

public interface IRepositorySources
{
    Task CreateRepository(ApplicationUser user, CreateRepositoryDto createRepositoryDto);
    Task UpdateRepository(ApplicationUser user, string userName, string repositoryTitle, UpdateRepositoryDto updateRepositoryDto);
    Task DeleteRepository(ApplicationUser user, string repositoryTitle);
    Task<FileSystemTreeNode> GetStructure(ApplicationUser user, string userName, string repositoryTitle);
    
    
    Task AddDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName);
    Task AddFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName);
    
    Task DeleteDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName);
    Task DeleteFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName);
    
    Task CopyDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName, string dirDestFullName);
    Task CopyFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName, string fileDestFullName);
    Task MoveDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullName, string dirDestFullName);
    Task MoveFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullName, string fileDestFullName);
    Task InsertDirectory(ApplicationUser user, string userName, string repositoryTitle, string parentDirPath, IFormFile archive);
    Task InsertFile(ApplicationUser user, string userName, string repositoryTitle, string parentDirPath, IFormFile file);

    Task<Stream> GetDirectory(ApplicationUser user, string userName, string repositoryTitle, string dirFullPath);
    Task<Stream> GetFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullPath);
    
    Task UpdateFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullPath, IFormFile file);
    Task UpdateFile(ApplicationUser user, string userName, string repositoryTitle, string fileFullPath, Stream fileStream);
}
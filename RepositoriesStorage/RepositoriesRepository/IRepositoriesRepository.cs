namespace RepositoriesStorage.RepositoriesRepository;

public interface IRepositoriesRepository
{
    Task CreateRepository(IRepositoryIdentity repositoryIdentity);
    Task UpdateRepository(IRepositoryIdentity repositoryIdentity, IRepositoryIdentity newRepositoryIdentity);
    Task DeleteRepository(IRepositoryIdentity repositoryIdentity);
    FileSystemTreeNode GetStructure(IRepositoryIdentity repositoryIdentity);
    
    Task AddDirectory(IRepositoryIdentity repositoryIdentity, string directoryPath);
    Task AddFile(IRepositoryIdentity repositoryIdentity, string filePath);

    Task DeleteDirectory(IRepositoryIdentity repositoryIdentity, string directoryPath);
    Task DeleteFile(IRepositoryIdentity repositoryIdentity, string filePath);

    Task MoveDirectory(IRepositoryIdentity repositoryIdentity, string sourceDirectoryPath, string destDirectoryPath);
    Task MoveFile(IRepositoryIdentity repositoryIdentity, string sourceFilePath, string destFilePath);
    Task CopyDirectory(IRepositoryIdentity repositoryIdentity, string sourceDirectoryPath, string destDirectoryPath);
    Task CopyFile(IRepositoryIdentity repositoryIdentity, string sourceFilePath, string destFilePath);

    Task InsertDirectory(IRepositoryIdentity repositoryIdentity, string parentDirectoryPath, Stream archiveStream);
    Task InsertFile(IRepositoryIdentity repositoryIdentity, string fileLocalPath, Stream fileStream);

    Task<Stream> GetDirectory(IRepositoryIdentity repositoryIdentity, string directoryPath);
    Task<Stream> GetFile(IRepositoryIdentity repositoryIdentity, string filePath);

    Task UpdateFile(IRepositoryIdentity repositoryIdentity, string filePath, Stream fileStream);
}
using RepositoryStoragesRealizations.DirectoryStorage.TempStorage;

namespace RepositoryStoragesRealizations.DirectoryStorage.RepositoriesStorage;

public interface IDirectoryRepositoriesStorage
{
    void CreateRepository(string userName, string repositoryTitle);
    void RenameRepository(string userName, string repositoryTitle, string newRepositoryTitle);

    void RenameUser(string userName, string newUserName);
    void DeleteUser(string userName);
    void DeleteRepository(string userName, string repositoryTitle);
    FileSystemTreeNode GetStructure(string userName, string repositoryTitle);
    void AddDirectory(string userName, string repositoryTitle, string dirLocalPath);
    void DeleteDirectory(string userName, string repositoryTitle, string dirLocalPath);
    void MoveDirectory(string userName, string repositoryTitle, string dirLocalPath, string newDirLocalPath);
    string GetDirectory(string userName, string repositoryTitle, string dirLocalPath, string archiveName);
    void InsertDirectory(string userName, string repositoryTitle, string parentDirLocalPath, string archiveFileFullPath);
    void AddFile(string userName, string repositoryTitle, string fileLocalPath);
    void DeleteFile(string userName, string repositoryTitle, string fileLocalPath);
    void MoveFile(string userName, string repositoryTitle, string fileLocalPath, string newFileLocalPath);
    byte[] GetFileSource(string userName, string repositoryTitle, string fileLocalPath);
    Task<byte[]> GetFileSourceAsync(string userName, string repositoryTitle, string fileLocalPath);
    void InsertFile(string userName, string repositoryTitle, string fileLocalPath, string insertableFileFullPath);
    void EditFile(string userName, string repositoryTitle, string fileLocalPath, byte[] source);
    Task EditFileAsync(string userName, string repositoryTitle, string fileLocalPath, byte[] source);
    
    public ITempStorage TempStorage { get; }
}
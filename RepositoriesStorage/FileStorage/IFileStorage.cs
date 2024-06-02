namespace RepositoriesStorage.FileStorage;

public interface IFileStorage
{
    Task CreateDirectory(string newDirectoryPath);
    Task CreateFile(string newFilePath);

    Task DeleteDirectory(string directoryPath);
    Task DeleteFile(string filePath);
    
    Task CopyDirectory(string sourceDirectoryPath, string destDirectoryPath);
    Task CopyFile(string sourceFilePath, string destFilePath);
    Task MoveDirectory(string sourceDirectoryPath, string destDirectoryPath);
    Task MoveFile(string sourceFilePath, string destFilePath);
    Task InsertDirectory(string destDirectoryPath, Stream archiveStream);
    Task InsertFile(string destFilePath, Stream fileStream);

    Task<Stream> GetDirectoryAsStream(string directoryPath);
    Task<Stream> GetFileAsStream(string filePath);

    Task UpdateFile(string filePath, Stream sourcesStream);
}
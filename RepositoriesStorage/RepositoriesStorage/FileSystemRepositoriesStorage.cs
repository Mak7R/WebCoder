using RepositoriesStorage.FilesManager;

namespace RepositoriesStorage.RepositoriesStorage;

public class FileSystemRepositoriesStorage : IRepositoriesStorage
{
    private readonly IFileSystemManager _fileSystemManager;
    private readonly string _storageDirectoryPath;
    private string ToFullPath(string path) => Path.Combine(_storageDirectoryPath, path);

    public FileSystemRepositoriesStorage(IFileSystemManager fileSystemManager, string storageDirectoryPath)
    {
        _fileSystemManager = fileSystemManager;
        _storageDirectoryPath = storageDirectoryPath;

        if (!Directory.Exists(_storageDirectoryPath)) Directory.CreateDirectory(_storageDirectoryPath);
    }
    
    public async Task CreateRepository(IRepositoryIdentity repositoryIdentity)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.CreateDirectory(fullRepositoryPath);
    }

    public async Task DeleteRepository(IRepositoryIdentity repositoryIdentity)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.DeleteDirectory(fullRepositoryPath);
    }
    
    public FileSystemTreeNode GetStructure(IRepositoryIdentity repositoryIdentity)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        if (!Directory.Exists(fullRepositoryPath)) throw new InvalidOperationException("Repository doesn't exists");
        
        FileSystemTreeNode root = new FileSystemTreeNode(fullRepositoryPath, "Repository");
        
        FillTree(root, fullRepositoryPath);

        return root;

        void FillTree(FileSystemTreeNode parent, string path)
        {
            var dirs = Directory.EnumerateDirectories(path);
            foreach (var dir in dirs)
            {
                var dirNode = new FileSystemTreeNode(dir.Split(Path.DirectorySeparatorChar).Last(), "Directory");
                parent.Children.Add(dirNode);
                FillTree(dirNode, dir);
            }
                
            var files = Directory.EnumerateFiles(path);
            foreach (var file in files)
            {
                parent.Children.Add(new FileSystemTreeNode(file.Split(Path.DirectorySeparatorChar).Last(), "File"));
            }
        }
    }

    public async Task AddDirectory(IRepositoryIdentity repositoryIdentity, string directoryPath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.CreateDirectory(Path.Combine(fullRepositoryPath, directoryPath));
    }

    public async Task AddFile(IRepositoryIdentity repositoryIdentity, string filePath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.CreateDirectory(Path.Combine(fullRepositoryPath, filePath));
    }

    public async Task DeleteDirectory(IRepositoryIdentity repositoryIdentity, string directoryPath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.DeleteDirectory(Path.Combine(fullRepositoryPath, directoryPath));
    }

    public async Task DeleteFile(IRepositoryIdentity repositoryIdentity, string filePath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.DeleteFile(Path.Combine(fullRepositoryPath, filePath));
    }

    public async Task MoveDirectory(IRepositoryIdentity repositoryIdentity, string sourceDirectoryPath, string destDirectoryPath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.MoveDirectory(
            Path.Combine(fullRepositoryPath, sourceDirectoryPath), 
            Path.Combine(fullRepositoryPath, destDirectoryPath));
    }

    public async Task MoveFile(IRepositoryIdentity repositoryIdentity, string sourceFilePath, string destFilePath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.MoveFile(
            Path.Combine(fullRepositoryPath, sourceFilePath), 
            Path.Combine(fullRepositoryPath, destFilePath));
    }

    public async Task CopyDirectory(IRepositoryIdentity repositoryIdentity, string sourceDirectoryPath, string destDirectoryPath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.CopyDirectory(
            Path.Combine(fullRepositoryPath, sourceDirectoryPath), 
            Path.Combine(fullRepositoryPath, sourceDirectoryPath));
    }

    public async Task CopyFile(IRepositoryIdentity repositoryIdentity, string sourceFilePath, string destFilePath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.CopyDirectory(
            Path.Combine(fullRepositoryPath, sourceFilePath), 
            Path.Combine(fullRepositoryPath, destFilePath));
    }
    
    public async Task InsertDirectory(IRepositoryIdentity repositoryIdentity, string parentDirectoryPath, Stream archiveStream)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.InsertDirectory(Path.Combine(fullRepositoryPath, parentDirectoryPath), archiveStream);
    }

    public async Task InsertFile(IRepositoryIdentity repositoryIdentity, string parentDirectoryPath, Stream fileStream)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.InsertFile(Path.Combine(fullRepositoryPath, parentDirectoryPath), fileStream);
    }

    public async Task<Stream> GetDirectory(IRepositoryIdentity repositoryIdentity, string directoryPath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        return await _fileSystemManager.GetDirectoryAsStream(Path.Combine(fullRepositoryPath, directoryPath));
    }

    public async Task<Stream> GetFile(IRepositoryIdentity repositoryIdentity, string filePath)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        return await _fileSystemManager.GetFileAsStream(Path.Combine(fullRepositoryPath, filePath));
    }

    public async Task UpdateFile(IRepositoryIdentity repositoryIdentity, string filePath, Stream fileStream)
    {
        var fullRepositoryPath = ToFullPath(repositoryIdentity.RepositoryName);
        await _fileSystemManager.UpdateFile(Path.Combine(fullRepositoryPath, filePath), fileStream);
    }
}
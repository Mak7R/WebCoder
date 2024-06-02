using RepositoriesStorage.Archiver;

namespace RepositoriesStorage.FileStorage;

public class FileStorage : IFileStorage
{
    private readonly IArchiver _archiver;

    public FileStorage(IArchiver archiver)
    {
        _archiver = archiver;
    }
    
    
    public Task CreateDirectory(string newDirectoryPath)
    {
        if (string.IsNullOrEmpty(newDirectoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(newDirectoryPath));

        Directory.CreateDirectory(newDirectoryPath);
        return Task.CompletedTask;
    }

    public async Task CreateFile(string newFilePath)
    {
        if (string.IsNullOrEmpty(newFilePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(newFilePath));

        await using var fs = new FileStream(newFilePath, FileMode.CreateNew);
        await fs.FlushAsync();
    }

    public Task DeleteDirectory(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

        Directory.Delete(directoryPath, true);
        return Task.CompletedTask;
    }

    public Task DeleteFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        File.Delete(filePath);
        return Task.CompletedTask;
    }

    public async Task CopyDirectory(string sourceDirectoryPath, string destDirectoryPath)
    {
        if (string.IsNullOrEmpty(sourceDirectoryPath) || string.IsNullOrEmpty(destDirectoryPath))
            throw new ArgumentException("Source and destination paths cannot be null or empty.");

        if (!Directory.Exists(sourceDirectoryPath))
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectoryPath}");

        Directory.CreateDirectory(destDirectoryPath);

        foreach (var file in Directory.GetFiles(sourceDirectoryPath, "*", SearchOption.AllDirectories))
        {
            var relativePath = file.Substring(sourceDirectoryPath.Length + 1);
            var destFile = Path.Combine(destDirectoryPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            File.Copy(file, destFile, true);
        }

        await Task.CompletedTask;
    }

    public Task CopyFile(string sourceFilePath, string destFilePath)
    {
        if (string.IsNullOrEmpty(sourceFilePath) || string.IsNullOrEmpty(destFilePath))
            throw new ArgumentException("Source and destination paths cannot be null or empty.");

        File.Copy(sourceFilePath, destFilePath, true);
        return Task.CompletedTask;
    }

    public Task MoveDirectory(string sourceDirectoryPath, string destDirectoryPath)
    {
        if (string.IsNullOrEmpty(sourceDirectoryPath) || string.IsNullOrEmpty(destDirectoryPath))
            throw new ArgumentException("Source and destination paths cannot be null or empty.");

        Directory.Move(sourceDirectoryPath, destDirectoryPath);
        return Task.CompletedTask;
    }

    public Task MoveFile(string sourceFilePath, string destFilePath)
    {
        if (string.IsNullOrEmpty(sourceFilePath) || string.IsNullOrEmpty(destFilePath))
            throw new ArgumentException("Source and destination paths cannot be null or empty.");

        File.Move(sourceFilePath, destFilePath);
        return Task.CompletedTask;
    }

    public async Task InsertDirectory(string destDirectoryPath, Stream archiveStream)
    {
        if (string.IsNullOrEmpty(destDirectoryPath))
            throw new ArgumentException("Destination directory path cannot be null or empty.", nameof(destDirectoryPath));
        
        if (archiveStream == null)
            throw new ArgumentNullException(nameof(archiveStream));

        await _archiver.Unarchive(archiveStream, destDirectoryPath);
    }

    public async Task InsertFile(string destFilePath, Stream fileStream)
    {
        if (string.IsNullOrEmpty(destFilePath))
            throw new ArgumentException("Destination file path cannot be null or empty.", nameof(destFilePath));

        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        await using var fs = new FileStream(destFilePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fs);
    }

    public async Task<Stream> GetDirectoryAsStream(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

        return await _archiver.Archive(directoryPath);
    }

    public async Task<Stream> GetFileAsStream(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        var memoryStream = new MemoryStream();
        await using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await fs.CopyToAsync(memoryStream);
        }
        
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    public async Task UpdateFile(string filePath, Stream sourceStream)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (sourceStream == null)
            throw new ArgumentNullException(nameof(sourceStream));

        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await sourceStream.CopyToAsync(fs);
    }
}
using RepositoryStoragesRealizations.DirectoryStorage.TempStorage;
using RepositoryStoragesRealizations.FileArchiver;

namespace RepositoryStoragesRealizations.DirectoryStorage.RepositoriesStorage;

// TODO LIST
// cache for archives and other
// cache controller
// old used repos to db
// check actual archives
// handle already exists repos

public class DirectoryRepositoriesStorage : IDirectoryRepositoriesStorage
{
    // dependencies
    private readonly string _rootDirectory;
    private readonly IFileArchiver _fileArchiver;
    private readonly ITempStorage _tempStorage;
    
    // constructors
    public DirectoryRepositoriesStorage(string rootDirectory, ITempStorage tempStorage, IFileArchiver fileArchiver)
    {
        _rootDirectory = rootDirectory;
        _fileArchiver = fileArchiver;
        _tempStorage = tempStorage;
        
        if (!Directory.Exists(_rootDirectory))
        {
            Directory.CreateDirectory(_rootDirectory);
        }
    }
    
    // repository methods
    public void CreateRepository(string userName, string repositoryTitle)
    {
        string userDir = Path.Combine(_rootDirectory, userName);

        if (!Directory.Exists(userDir)) Directory.CreateDirectory(userDir);

        string repositoryFullPath = Path.Combine(userDir, repositoryTitle);

        if (!Directory.Exists(repositoryFullPath)) Directory.CreateDirectory(repositoryFullPath);
        else throw new InvalidOperationException("Repository already exists");
    }

    public void RenameRepository(string userName, string repositoryTitle, string newRepositoryTitle)
    {
        string repositoryFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle);
        if (!Directory.Exists(repositoryFullPath)) throw new InvalidOperationException("Repository doesn't exists");
        
        string newRepositoryFullPath = Path.Combine(_rootDirectory, userName, newRepositoryTitle);
        if (Directory.Exists(newRepositoryFullPath)) throw new InvalidOperationException("Repository doesn't exists");
        Directory.Move(repositoryFullPath, newRepositoryFullPath);
    }

    public void RenameUser(string userName, string newUserName)
    {
        string userDir = Path.Combine(_rootDirectory, userName);
        if (!Directory.Exists(userDir)) return;
        string newUserDir = Path.Combine(_rootDirectory, newUserName);
        Directory.Move(userDir, newUserDir);
    }

    public void DeleteUser(string userName)
    {
        string userDir = Path.Combine(_rootDirectory, userName);
        
        if (Directory.Exists(userDir)) Directory.Delete(userDir, true);
    }
    
    public void DeleteRepository(string userName, string repositoryTitle)
    {
        string userDir = Path.Combine(_rootDirectory, userName);

        if (!Directory.Exists(userDir)) return;

        string repositoryFullPath = Path.Combine(userDir, repositoryTitle);
        
        if (Directory.Exists(repositoryFullPath)) Directory.Delete(repositoryFullPath, true);
    }

    public FileSystemTreeNode GetStructure(string userName, string repositoryTitle)
    {
        string repositoryFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle);
        if (!Directory.Exists(repositoryFullPath)) throw new InvalidOperationException("Repository doesn't exists");
        
        FileSystemTreeNode root = new FileSystemTreeNode(repositoryTitle, "Repository");
        
        FillTree(root, repositoryFullPath);

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

    // directory methods
    public void AddDirectory(string userName, string repositoryTitle, string dirLocalPath)
    {
        var newDirFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, dirLocalPath);

        if (Directory.Exists(newDirFullPath)) throw new InvalidOperationException("Directory already exists");

        Directory.CreateDirectory(newDirFullPath);
    }

    public void DeleteDirectory(string userName, string repositoryTitle, string dirLocalPath)
    {
        var dirFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, dirLocalPath);
        
        if (!Directory.Exists(dirFullPath)) throw new InvalidOperationException("Directory doesn't exists");
        
        Directory.Delete(dirFullPath, true);
    }

    public void MoveDirectory(string userName, string repositoryTitle, string dirLocalPath, string newDirLocalPath)
    {
        var projectPath = Path.Combine(_rootDirectory, userName, repositoryTitle);
        
        var dirFullPath = Path.Combine(projectPath, dirLocalPath);
        if (!Directory.Exists(dirFullPath)) throw new InvalidOperationException("Directory doesn't exists");
        
        var newDirFullPath = Path.Combine(projectPath, newDirLocalPath);
        if (Directory.Exists(newDirFullPath))
            throw new InvalidOperationException("Directory with this name already exists");
        
        Directory.Move(dirFullPath, newDirFullPath);
    }

    public string GetDirectory(string userName, string repositoryTitle, string dirLocalPath, string archiveName)
    {
        var dirFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, dirLocalPath);
        if (!Directory.Exists(dirFullPath)) throw new InvalidOperationException("Directory doesn't exists");

        var dirInfo = _tempStorage.Allocate();
        var archiveFileFullPath = Path.Combine(dirInfo.FullPath, archiveName);
        
        _fileArchiver.Archive(dirFullPath, archiveFileFullPath);
        return archiveFileFullPath;
    }

    public void InsertDirectory(string userName, string repositoryTitle, string parentDirLocalPath, string archiveFileFullPath)
    {
        var parentDirFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, parentDirLocalPath);
        if (!Directory.Exists(parentDirFullPath)) throw new InvalidOperationException("Parent directory doesn't exists");
        
        if (!File.Exists(archiveFileFullPath)) throw new InvalidOperationException("Archive doesn't exists");

        var allocDirInfo = _tempStorage.Allocate();
        try
        {
            _fileArchiver.Unarchive(archiveFileFullPath, allocDirInfo.FullPath);

            // TODO handle already exists repo and files
            foreach (var dir in Directory.EnumerateDirectories(allocDirInfo.FullPath))
            {
                var name = dir.Split(Path.DirectorySeparatorChar).Last();
                Directory.Move(dir, Path.Combine(parentDirFullPath, name));
            }

            foreach (var file in Directory.EnumerateFiles(allocDirInfo.FullPath))
            {
                var name = file.Split(Path.DirectorySeparatorChar).Last();
                File.Move(file, Path.Combine(parentDirFullPath, name));
            }
        }
        finally
        {
            _tempStorage.Deallocate(allocDirInfo.Id);
        }
    }
    
    // files methods
    public void AddFile(string userName, string repositoryTitle, string fileLocalPath)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (File.Exists(fileFullPath)) throw new InvalidOperationException("File with this name already exists");

        File.Create(fileFullPath);
    }

    public void DeleteFile(string userName, string repositoryTitle, string fileLocalPath)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (!File.Exists(fileFullPath)) throw new InvalidOperationException("File doesn't exists");
        File.Delete(fileFullPath);
    }

    public void MoveFile(string userName, string repositoryTitle, string fileLocalPath, string newFileLocalPath)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (!File.Exists(fileFullPath)) throw new InvalidOperationException("File doesn't exists");

        var newFileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, newFileLocalPath);
        if (File.Exists(newFileFullPath)) throw new InvalidOperationException("File with this name already exists");
        
        File.Move(fileFullPath, newFileFullPath);
    }

    public byte[] GetFileSource(string userName, string repositoryTitle, string fileLocalPath)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (!File.Exists(fileFullPath)) throw new InvalidOperationException("File doesn't exists");

        return File.ReadAllBytes(fileFullPath);
    }

    public async Task<byte[]> GetFileSourceAsync(string userName, string repositoryTitle, string fileLocalPath)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (!File.Exists(fileFullPath)) throw new InvalidOperationException("File doesn't exists");

        return await File.ReadAllBytesAsync(fileFullPath);
    }

    public void InsertFile(string userName, string repositoryTitle, string fileLocalPath, string insertableFileFullPath)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (File.Exists(fileFullPath)) throw new InvalidOperationException("File with this name already exists");

        if (!File.Exists(insertableFileFullPath)) throw new InvalidOperationException("File doesn't exists");
        
        File.Copy(insertableFileFullPath, fileFullPath);
    }

    public void EditFile(string userName, string repositoryTitle, string fileLocalPath, byte[] source)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (!File.Exists(fileFullPath)) throw new InvalidOperationException("File doesn't exists");
        
        File.WriteAllBytes(fileFullPath, source);
    }
    
    public async Task EditFileAsync(string userName, string repositoryTitle, string fileLocalPath, byte[] source)
    {
        var fileFullPath = Path.Combine(_rootDirectory, userName, repositoryTitle, fileLocalPath);
        if (!File.Exists(fileFullPath)) throw new InvalidOperationException("File doesn't exists");
        
        await File.WriteAllBytesAsync(fileFullPath, source);
    }

    public ITempStorage TempStorage => _tempStorage;
}
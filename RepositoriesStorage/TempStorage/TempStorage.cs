namespace RepositoriesStorage.TempStorage;


public class TempStorage : ITempStorage
{
    private readonly string _rootDirectory;
    private const int MaxAttempts = 10;

    private readonly List<AllocatedDirectoryInfo> _allocatedDirectoryInfos = [];
    private static string GeneratePath(string rootDirectory, Guid id) => Path.Combine(rootDirectory, id.ToString());

    private static readonly int GuidLength = Guid.Empty.ToString().Length;
    public AllocatedDirectoryInfo? GetByPath(string path)
    {
        var id = Guid.Parse(path.AsSpan(_rootDirectory.Length + 1, GuidLength));
        return _allocatedDirectoryInfos.FirstOrDefault(d => d.Id == id);
    }
    
    public TempStorage(string rootDirectory)
    {
        _rootDirectory = rootDirectory;

        if (!Directory.Exists(_rootDirectory)) Directory.CreateDirectory(_rootDirectory);
    }

    public string GetDirectoryPath(Guid id) => GeneratePath(_rootDirectory, id);
    
    public AllocatedDirectoryInfo GetDirectoryInfo(Guid id)
    {
        var directoryInfo = _allocatedDirectoryInfos.FirstOrDefault(d => d.Id == id);
        if (directoryInfo is null) throw new InvalidOperationException("Directory doesn't exists");
        return directoryInfo;
    } 
    public AllocatedDirectoryInfo Allocate()
    {
        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var id = Guid.NewGuid();
            var allocatedDirPath = GeneratePath(_rootDirectory, id);

            if (Directory.Exists(allocatedDirPath)) continue;
            
            Directory.CreateDirectory(allocatedDirPath);
            
            var allocDirectoryInfo = new AllocatedDirectoryInfo(id, allocatedDirPath);
            _allocatedDirectoryInfos.Add(allocDirectoryInfo);
            
            return allocDirectoryInfo;
        }

        throw new InvalidOperationException("Failed to allocate a unique directory after multiple attempts.");
    }
    
    public void Deallocate(Guid id)
    {
        var directoryInfo = _allocatedDirectoryInfos.FirstOrDefault(d => d.Id == id);

        if (directoryInfo == null) return;
        
        _allocatedDirectoryInfos.Remove(directoryInfo);
        directoryInfo.Release();
    }

    public void Deallocate(Func<AllocatedDirectoryInfo, bool> predicate)
    {
        foreach (var directoryInfo in _allocatedDirectoryInfos.Where(predicate).ToList())
        {
            _allocatedDirectoryInfos.Remove(directoryInfo);
            directoryInfo.Release();
        }
    }
}
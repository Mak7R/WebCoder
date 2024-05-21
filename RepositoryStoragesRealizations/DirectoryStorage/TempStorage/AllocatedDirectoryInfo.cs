namespace RepositoryStoragesRealizations.DirectoryStorage.TempStorage;

public class AllocatedDirectoryInfo
{
    public Guid Id { get; }
    public string FullPath { get; }
        
    public bool Exists => Directory.Exists(FullPath);
    public DateTime AllocatedDateTime => Directory.GetCreationTime(FullPath);
    public DateTime LastWriteDataTime => Directory.GetLastWriteTime(FullPath);
    public DateTime LastAccessDateTime => Directory.GetLastAccessTime(FullPath);

    public AllocatedDirectoryInfo(Guid id, string fullPath)
    {
        Id = id;
        FullPath = fullPath;
    }

    public void Release()
    {
        if (Exists) Directory.Delete(FullPath, true);
    }
}
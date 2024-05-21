namespace RepositoriesStorage.TempStorage;

public interface ITempStorage
{
    AllocatedDirectoryInfo? GetByPath(string path);
    string GetDirectoryPath(Guid id);
    AllocatedDirectoryInfo Allocate();
    void Deallocate(Guid id);
    void Deallocate(Func<AllocatedDirectoryInfo, bool> predicate);
}
namespace RepositoriesStorage.Archiver;

public interface IArchiver
{
    Task<Stream> Archive(string path);
    Task Unarchive(Stream archive, string path);
}
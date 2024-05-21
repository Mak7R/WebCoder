namespace RepositoryStoragesRealizations.FileArchiver;

public interface IFileArchiver
{
    /// <summary>
    /// Creates from directory by path <b>sourcePath</b> archive by path <b>archivePath</b>.
    /// </summary>
    /// <param name="sourcePath">Path to source file or directory</param>
    /// <param name="archivePath">Path to archive file</param>
    public void Archive(string sourcePath, string archivePath);
    
    /// <summary>
    /// Creates from archive by path <b>archivePath</b> directory by path <b>sourcePath</b>.
    /// </summary>
    /// <param name="archivePath">Path to archive file</param>
    /// <param name="sourcePath">Path to source file or directory</param>
    public void Unarchive(string archivePath, string sourcePath);
}
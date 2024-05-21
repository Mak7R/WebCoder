using System.IO.Compression;

namespace RepositoryStoragesRealizations.FileArchiver;

public class ZipFileArchiver : IFileArchiver
{
    public void Archive(string sourcePath, string archivePath)
    {
        if (File.Exists(archivePath))
        {
            File.Delete(archivePath);
        }
        ZipFile.CreateFromDirectory(sourcePath, archivePath);
    }

    public void Unarchive(string archivePath, string sourcePath)
    {
        if (Directory.Exists(sourcePath))
        {
            Directory.Delete(sourcePath, true);
        }

        ZipFile.ExtractToDirectory(archivePath, sourcePath);
    }
}
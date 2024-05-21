using System.IO.Compression;

namespace RepositoriesStorage.Archiver;

public class ZipArchiver : IArchiver
{
    public Task<Stream> Archive(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");

        var memoryStream = new MemoryStream();
        
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                var entryName = file.Substring(path.Length + 1);
                var entry = archive.CreateEntryFromFile(file, entryName);
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return Task.FromResult<Stream>(memoryStream);
    }

    public async Task Unarchive(Stream archiveStream, string outputPath)
    {
        if (archiveStream == null)
            throw new ArgumentNullException(nameof(archiveStream));

        if (string.IsNullOrEmpty(outputPath))
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read);
        foreach (var entry in archive.Entries)
        {
            var destinationPath = Path.Combine(outputPath, entry.FullName);
            var destinationDir = Path.GetDirectoryName(destinationPath);
                
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);
                
            if (string.IsNullOrEmpty(entry.Name)) continue; // continue for directories

            await using var entryStream = entry.Open();
            await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await entryStream.CopyToAsync(fileStream);
        }
    }
}
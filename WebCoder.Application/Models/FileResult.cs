namespace WebCoder.Application.Models;

public class FileResult(string fileName, Stream fileStream)
{
    public string FileName { get; } = fileName;
    public Stream FileStream { get; } = fileStream;
}
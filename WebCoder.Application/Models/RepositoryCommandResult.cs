namespace WebCoder.Application.Models;

public class RepositoryCommandResult
{
    public bool IsSuccessful { get; }
    
    public IEnumerable<string> Errors { get; }
    
    public object? ResultObject { get; }

    public RepositoryCommandResult(bool isSuccessful, IEnumerable<string> errors, object? resultObject = null)
    {
        IsSuccessful = isSuccessful;
        Errors = errors;
        ResultObject = resultObject;
    }
}
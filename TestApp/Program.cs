

using WebCoder.Application.Services;

namespace TestApp;

class Program
{
    static void Main(string[] args)
    {
        var commandHandler = new RepositoryCommandHandler();

        var result = commandHandler.Execute("user", "repo", "add 3 5 6");

        var obj = result.ResultObject;
    }
}
using Microsoft.AspNetCore.Http;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.Application.Models;

namespace WebCoder.Application.Services;



public class RepositoryCommandHandler : IRepositoryCommandHandler
{
    private readonly IRepositorySources _repositorySources;

    public RepositoryCommandHandler(IRepositorySources repositorySources)
    {
        _repositorySources = repositorySources;
    }


    private class InvalidCommandSyntaxException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public InvalidCommandSyntaxException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
    
    private struct CommandModel
    {
        public string Command { get; set; }
        public string[] Args { get; set; }

        public static CommandModel Parse(string command)
        {
            var tokens = Tokenize(command);
            ValidateSyntax(tokens);

            return new CommandModel
            {
                Command = tokens[0],
                Args = tokens.GetRange(1, tokens.Count - 1).ToArray()
            };
        }

        private static List<string> Tokenize(string command)
        {
            var tokens = new List<string>();
            bool inDoubleQuotes = false;
            bool inSingleQuotes = false;
            int start = 0;

            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] == '"')
                {
                    if (!inSingleQuotes)
                        inDoubleQuotes = !inDoubleQuotes;
                }
                else if (command[i] == '\'')
                {
                    if (!inDoubleQuotes)
                        inSingleQuotes = !inSingleQuotes;
                }
                else if (command[i] == ' ' && !inDoubleQuotes && !inSingleQuotes)
                {
                    if (i > start)
                    {
                        string token = command.Substring(start, i - start);
                        if ((token.StartsWith("\"") && token.EndsWith("\"")) || (token.StartsWith("'") && token.EndsWith("'")))
                        {
                            token = token.Substring(1, token.Length - 2);
                        }
                        tokens.Add(token);
                    }
                    start = i + 1;
                }
            }

            if (start < command.Length)
            {
                string token = command.Substring(start);
                if ((token.StartsWith("\"") && token.EndsWith("\"")) || (token.StartsWith("'") && token.EndsWith("'")))
                {
                    token = token.Substring(1, token.Length - 2);
                }
                tokens.Add(token);
            }

            return tokens;
        }

        private static void ValidateSyntax(List<string> tokens)
        {
            var errors = new List<string>();
            bool inDoubleQuotes = false;
            bool inSingleQuotes = false;

            if (tokens.Count == 0)
            {
                errors.Add("No command provided.");
            }
            else
            {
                for (int i = 1; i < tokens.Count; i++)
                {
                    if (tokens[i].StartsWith("'") || tokens[i].StartsWith("\""))
                    {
                        if (inDoubleQuotes && tokens[i].StartsWith("'") || inSingleQuotes && tokens[i].StartsWith("\""))
                        {
                            continue;
                        }
                        if (tokens[i].StartsWith("\""))
                        {
                            inDoubleQuotes = true;
                        }
                        else if (tokens[i].StartsWith("'"))
                        {
                            inSingleQuotes = true;
                        }
                    }

                    if (tokens[i].EndsWith("'") || tokens[i].EndsWith("\""))
                    {
                        if ((inDoubleQuotes && tokens[i].EndsWith("\"")) || (inSingleQuotes && tokens[i].EndsWith("'")))
                        {
                            continue;
                        }
                        if (tokens[i].EndsWith("\""))
                        {
                            inDoubleQuotes = false;
                        }
                        else if (tokens[i].EndsWith("'"))
                        {
                            inSingleQuotes = false;
                        }
                    }
                }

                if (inDoubleQuotes)
                {
                    errors.Add("Unmatched double quote at end of command.");
                }
                if (inSingleQuotes)
                {
                    errors.Add("Unmatched single quote at end of command.");
                }
            }

            if (errors.Count > 0)
            {
                throw new InvalidCommandSyntaxException(errors);
            }
        }
    }


    private static readonly string[] DirectoryAttributes = ["FOLDER", "DIRECTORY", "-D"];
    private static readonly string[] FileAttributes = ["FILE", "-F"];
    
    private static class SupportedOperations
    {
        public const string Add = "ADD";
        public const string Delete = "DELETE";
        public const string Insert = "INSERT";
        public const string Copy = "COPY";
        public const string Move = "MOVE";
        public const string Load = "LOAD";
    }
    
    
    public async Task<RepositoryCommandResult> Execute(ApplicationUser user, string userName, string repositoryTitle, string command, IFormFile? sources = null)
    {
        CommandModel commandModel;
        try
        {
            commandModel = CommandModel.Parse(command);
        }
        catch (InvalidCommandSyntaxException ex)
        {
            return new RepositoryCommandResult(false, ex.Errors);
        }

        switch (commandModel.Command.ToUpper())
        {
            case SupportedOperations.Add:
                if (commandModel.Args.Length != 2)
                    return new RepositoryCommandResult(false, ["Invalid arguments count"]);
                
                if (DirectoryAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.AddDirectory(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]));
                }
                else if (FileAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.AddFile(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]));
                }
                else
                {
                    return new RepositoryCommandResult(false, ["Invalid arguments attributes"]);
                }
                break;
            case SupportedOperations.Delete:
                if (commandModel.Args.Length != 2)
                    return new RepositoryCommandResult(false, ["Invalid arguments count"]);
                
                if (DirectoryAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.DeleteDirectory(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]));
                }
                else if (FileAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.DeleteFile(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]));
                }
                else
                {
                    return new RepositoryCommandResult(false, ["Invalid arguments attributes"]);
                }
                break;
            case SupportedOperations.Copy:
                if (commandModel.Args.Length != 3)
                    return new RepositoryCommandResult(false, ["Invalid arguments count"]);
                
                if (DirectoryAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.CopyDirectory(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]), ParsePath(commandModel.Args[2]));
                }
                else if (FileAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.CopyDirectory(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]), ParsePath(commandModel.Args[2]));
                }
                else
                {
                    return new RepositoryCommandResult(false, ["Invalid arguments attributes"]);
                }
                break;
            case SupportedOperations.Move:
                if (commandModel.Args.Length != 3)
                    return new RepositoryCommandResult(false, ["Invalid arguments count"]);
                
                if (DirectoryAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.MoveDirectory(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]), ParsePath(commandModel.Args[2]));
                }
                else if (FileAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.MoveFile(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]), ParsePath(commandModel.Args[2]));
                }
                else
                {
                    return new RepositoryCommandResult(false, ["Invalid arguments attributes"]);
                }
                break;
            case SupportedOperations.Insert:
            {
                if (commandModel.Args.Length != 2)
                    return new RepositoryCommandResult(false, ["Invalid arguments count"]);

                if (sources is null)
                    return new RepositoryCommandResult(false, ["Sources was null"]);
                
                if (DirectoryAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.InsertDirectory(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]), sources);
                }
                else if (FileAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    await _repositorySources.InsertFile(user, userName, repositoryTitle, ParsePath(commandModel.Args[1]), sources);
                }
                else
                {
                    return new RepositoryCommandResult(false, ["Invalid arguments attributes"]);
                }
                break;
            }
            case SupportedOperations.Load:
                if (commandModel.Args.Length != 2)
                    return new RepositoryCommandResult(false, ["Invalid arguments count"]);

                var path = ParsePath(commandModel.Args[1]);
                if (DirectoryAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    var archive = await _repositorySources.GetDirectory(user, userName, repositoryTitle, path);
                    return new RepositoryCommandResult(true, Array.Empty<string>(), new FileResult(path.Split(Path.DirectorySeparatorChar).Last() + ".zip", archive));
                }
                else if (FileAttributes.Contains(commandModel.Args[0].ToUpper()))
                {
                    var file = await _repositorySources.GetFile(user, userName, repositoryTitle, path);
                    return new RepositoryCommandResult(true, Array.Empty<string>(), new FileResult(path.Split(Path.DirectorySeparatorChar).Last(), file));
                }
                else
                {
                    return new RepositoryCommandResult(false, ["Invalid arguments attributes"]);
                }
                break;
            default:
                return new RepositoryCommandResult(false, ["Unknown command"]);
        }
        
        return new RepositoryCommandResult(true, Array.Empty<string>());

        string ParsePath(string userPath)
        {
            userPath = userPath.Replace('/', Path.DirectorySeparatorChar);
            userPath = userPath.StartsWith(Path.DirectorySeparatorChar) ? userPath.Substring(1) : userPath;
            return userPath.StartsWith(repositoryTitle + Path.DirectorySeparatorChar) ? userPath.Substring(repositoryTitle.Length + 1) : userPath;
        }
    }
}
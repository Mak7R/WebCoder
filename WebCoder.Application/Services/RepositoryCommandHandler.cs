using Microsoft.AspNetCore.Http;
using WebCoder.Application.Interfaces;
using WebCoder.Application.Models;

namespace WebCoder.Application.Services;



public class RepositoryCommandHandler : IRepositoryCommandHandler
{
    private readonly IRepositoryFilesService _repositoryFilesService;

    public RepositoryCommandHandler(IRepositoryFilesService repositoryFilesService)
    {
        _repositoryFilesService = repositoryFilesService;
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
    
    
    public RepositoryCommandResult Execute(string userName, string repositoryTitle, string command, IFormFile? sources = null)
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
            case "ADD":
                
                break;
            case "DELETE":
                break;
            default:
                return new RepositoryCommandResult(false, ["Unknown command"]);
        }
        
        return new RepositoryCommandResult(true, Array.Empty<string>());
    }
}
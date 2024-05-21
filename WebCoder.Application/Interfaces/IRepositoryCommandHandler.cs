using Microsoft.AspNetCore.Http;
using WebCoder.Application.Models;

namespace WebCoder.Application.Interfaces;

public interface IRepositoryCommandHandler
{
    public RepositoryCommandResult Execute(string userName, string repositoryTitle, string command, IFormFile? sources = null);
}
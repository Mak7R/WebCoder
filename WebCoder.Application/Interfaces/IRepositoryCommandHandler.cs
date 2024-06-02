using Microsoft.AspNetCore.Http;
using WebCoder.Application.Identity;
using WebCoder.Application.Models;

namespace WebCoder.Application.Interfaces;

public interface IRepositoryCommandHandler
{
    public Task<RepositoryCommandResult> Execute(ApplicationUser user, string userName, string repositoryTitle, string command, IFormFile? sources = null);
}
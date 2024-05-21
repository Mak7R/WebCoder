using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebCoder.Application.DTOs.ProjectRepository;

public class CommandDto
{
    [Display(Name = "Command")]
    public string? Command { get; set; }
    
    [Display(Name = "File which is used in command")]
    public IFormFile? Sources { get; set; }
}
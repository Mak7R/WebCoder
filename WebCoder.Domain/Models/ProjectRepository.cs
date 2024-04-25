using System.ComponentModel.DataAnnotations;
using WebCoder.Domain.Rules;

namespace WebCoder.Domain.Models;

public class ProjectRepository
{
    [Key] public Guid Id { get; set; }
    [StringLength(DataSizes.ProjectRepository.MaxTitleLength)] public string Title { get; set; } = string.Empty;
    [StringLength(DataSizes.ApplicationUser.MaxUserNameLength)] public string OwnerUserName { get; set; } = string.Empty;
    
    public bool IsPublic { get; set; }
    
    [StringLength(DataSizes.ProjectRepository.MaxAboutLength)] public string? About { get; set; }
    public DateTime CreationDate { get; set; }
    
    // public File (.zip)

    // public List<User> followers 
}
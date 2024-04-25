using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebCoder.Application.Identity;
using WebCoder.Domain.Rules;

namespace WebCoder.Infrastructure.Entities;

public class ProjectRepositoryEntity
{
    [Key] public Guid Id { get; set; }
    
    [ForeignKey(nameof(Owner))]
    public Guid OwnerId { get; set; }
    
    public ApplicationUser Owner { get; set; }
    
    [StringLength(DataSizes.ProjectRepository.MaxTitleLength)] public string Title { get; set; }
    
    public bool IsPublic { get; set; }
    
    [StringLength(DataSizes.ProjectRepository.MaxAboutLength)] public string? About { get; set; }
    public DateTime CreationDate { get; set; }
}
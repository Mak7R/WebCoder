using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static WebCoder.Domain.Rules.DataSizes.ProjectRepository;

namespace WebCoder.Application.DTOs.ProjectRepository;

public class UpdateRepositoryDto
{
    [Display(Name = "Title")]
    [Required(ErrorMessage = "Title is required")]
    [StringLength(MaxTitleLength, MinimumLength = MinTitleLength, ErrorMessage = "Title length must be from 4 to 64 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Display(Name = "About repository")]
    [StringLength(MaxAboutLength, ErrorMessage = "About length must be less than 1024 characters")]
    public string? About { get; set; }
    
    [Display(Name = "Is repository public")] 
    public bool IsPublic { get; set; }
}
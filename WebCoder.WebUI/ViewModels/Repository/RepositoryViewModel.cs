using WebCoder.WebUI.Enums;

namespace WebCoder.WebUI.ViewModels.Repository;

public class RepositoryViewModel
{
    public Guid Id { get; set; }
    
    public UserPermission UserPermission { get; set; } 
    
    public string Title { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    
    public string Owner { get; set; } = string.Empty;
    public string? About { get; set; } = string.Empty;
}
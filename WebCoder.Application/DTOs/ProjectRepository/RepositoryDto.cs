namespace WebCoder.Application.DTOs.ProjectRepository;

public class RepositoryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OwnerUserName { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string? About { get; set; }
    public DateTime CreationDate { get; set; }
}
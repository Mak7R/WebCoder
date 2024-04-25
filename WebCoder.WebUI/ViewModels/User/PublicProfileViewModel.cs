namespace WebCoder.WebUI.ViewModels.User;

public class PublicProfileViewModel
{
    public string? Username { get; set; }
    public string? Profession { get; set; }
    public string? Email { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
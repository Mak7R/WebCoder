namespace WebCoder.WebUI.ViewModels.User;

public class ProfileViewModel
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Username { get; set; }
    public string? Profession { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
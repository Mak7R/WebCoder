using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static WebCoder.Domain.Rules.DataSizes.ApplicationUser;
namespace WebCoder.Application.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    [StringLength(MaxNameLength)] public string Name { get; set; } = string.Empty;
    [StringLength(MaxSurnameLength)] public string Surname { get; set; } = string.Empty;
    [StringLength(MaxProfessionLength)] public string? Profession { get; set; }

    [StringLength(MaxUserNameLength)] public override string? UserName { get; set; }
}
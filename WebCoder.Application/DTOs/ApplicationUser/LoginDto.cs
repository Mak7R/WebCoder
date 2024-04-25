using System.ComponentModel.DataAnnotations;
using static WebCoder.Domain.Rules.DataSizes.ApplicationUser;

namespace WebCoder.Application.DTOs.ApplicationUser;

public class LoginDto
{
    [Display(Name = "Email or username")]
    [Required(ErrorMessage = "Email or username is required")]
    [StringLength(MaxEmailLength > MaxUserNameLength ? MaxEmailLength : MaxUserNameLength, ErrorMessage = "Email length is invalid")]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
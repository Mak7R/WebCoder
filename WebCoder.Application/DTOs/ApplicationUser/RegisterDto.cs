using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using static WebCoder.Domain.Rules.DataSizes.ApplicationUser;


namespace WebCoder.Application.DTOs.ApplicationUser;

public class RegisterDto
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(MaxNameLength, ErrorMessage = "Name length must have less than 64 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Surname")]
    [Required(ErrorMessage = "Surname is required")]
    [StringLength(MaxSurnameLength, ErrorMessage = "Surname length must have less than 64 characters")]
    public string Surname { get; set; } = string.Empty;
    
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required")]
    [StringLength(MaxUserNameLength, MinimumLength = 4, ErrorMessage = "Username length must have from 4 to 64 characters")]
    [Remote(action: "IsUserNameFree", controller: "Account", ErrorMessage = "This username already taken")]
    public string Username { get; set; } = string.Empty;
    
    [Display(Name = "Profession")]
    [StringLength(MaxProfessionLength, ErrorMessage = "Profession must be shorter")]
    public string? Profession { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email is required")]
    [StringLength(MaxEmailLength, ErrorMessage = "Email length is invalid ")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [Remote(action: "IsEmailFree", controller: "Account", ErrorMessage = "This email already registered")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone number")]
    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Phone is invalid")]
    public string Phone { get; set; } = string.Empty;
    
    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Display(Name = "Confirm password")]
    [Required(ErrorMessage = "Confirm password is required")]
    [Compare(nameof(Password), ErrorMessage = "Confirm password must be same with password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}
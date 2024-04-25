using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCoder.Application.DTOs.ApplicationUser;
using WebCoder.Application.Identity;
using WebCoder.WebUI.Extensions;
using WebCoder.WebUI.ViewModels.User;

namespace WebCoder.WebUI.Controllers;

[Controller]
[Route("[controller]/[action]")]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<ApplicationRole> roleManager)
    : Controller
{
    [AllowAnonymous]
    [HttpGet("/register")]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromForm] RegisterDto registerDto, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }

        var newUser = new ApplicationUser
        {
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            UserName = registerDto.Username,
            Email = registerDto.Email,
            PhoneNumber = registerDto.Phone,
        };

        var result = await userManager.CreateAsync(newUser, registerDto.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newUser, "User");
            await signInManager.SignInAsync(newUser, isPersistent: true);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl);
            return LocalRedirect("/");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return View(registerDto);
        }
    }
    
    [AllowAnonymous]
    [HttpGet("/login")]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        var user = await userManager.FindByEmailAsync(loginDto.EmailOrUsername);
        if (user is null)
        {
            ModelState.AddModelError("Login", "Invalid email or password");
            return View(loginDto);
        }
        
        var result = await signInManager.PasswordSignInAsync(user.UserName ?? string.Empty, loginDto.Password, isPersistent: true, lockoutOnFailure: false);
        
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl);
            return LocalRedirect("/");
        }
        
        ModelState.AddModelError("Login", "Invalid email or password");
        return View(loginDto);
    }

    [Authorize]
    [Route("/logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return LocalRedirect("/");
    }

    [Authorize]
    [HttpGet("/profile")]
    public async Task<IActionResult> Profile()
    {
        var user = await userManager.GetUserAsync(User);
        
        if (user == null)
        {
            return RedirectToAction("Login","Account", new {ReturnUrl = "/profile"});
        }
        
        var userRoles = await userManager.GetRolesAsync(user);

        var userProfile = new ProfileViewModel
        {
            Name = user.Name,
            Surname = user.Surname,
            Username = user.UserName,
            Profession = user.Profession,
            Email = user.Email ?? string.Empty,
            Phone = user.PhoneNumber ?? string.Empty,
            Roles = userRoles
        };
        return View(userProfile);
    }
    
    [AllowAnonymous]
    [HttpGet("/user/{userName}")]
    public async Task<IActionResult> PublicProfile(string userName)
    {
        if (User.Identity?.Name == userName)
        {
            return RedirectToAction("Profile");
        }
        
        var user = await userManager.FindByNameAsync(userName);
        
        if (user == null) return this.NotFoundView("User was not found \ud83d\ude1e");
        
        var userRoles = await userManager.GetRolesAsync(user);
        
        var userProfile = new PublicProfileViewModel
        {
            Username = user.UserName,
            Profession = user.Profession,
            Email = user.Email ?? string.Empty,
            Roles = userRoles
        };
        
        return View(userProfile);
    }
    
    [AllowAnonymous]
    public async Task<IActionResult> IsEmailFree(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return Json(user == null);
    }
    
    [AllowAnonymous]
    public async Task<IActionResult> IsUserNameFree(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        return Json(user == null);
    }

    [AllowAnonymous]
    [Route("[action]")]
    public IActionResult AccessDenied(string? returnUrl)
    {
        return View("AccessDenied", returnUrl);
    }
}
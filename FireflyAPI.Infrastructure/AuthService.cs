using System.Security.Claims;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using FireflyAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure;

public class AuthService : IAuthService
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly SignInManager<AppIdentityUser> _signInManager;

    public AuthService(UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<Guid?> AuthenticateWithGoogleAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        
        if (info == null) 
            return null;

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

        if (signInResult.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var appUser = await _userManager.FindByEmailAsync(email);

            return appUser.Id;
        }

        var newUser = new AppIdentityUser
        {
            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
            UserName = info.Principal.FindFirstValue(ClaimTypes.Email)
           
        };

        var createResult = await _userManager.CreateAsync(newUser);

        if (!createResult.Succeeded)
            throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
        
        await _userManager.AddLoginAsync(newUser, info);

       // await _userManager.AddToRoleAsync(newUser, Roles.User);
        await _signInManager.SignInAsync(newUser, true);

        return newUser.Id;
    }

    public async Task<Guid?> LoginAsync(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);

        if (appUser == null)
            return null;

        var result = await _signInManager.PasswordSignInAsync(appUser, password, true, false);

        if (!result.Succeeded)
            return null;

        return appUser.Id;
    }

    public async Task<Guid?> RegisterAsync(User user, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser != null)
            throw new InvalidOperationException("User already exists");
        
        var newUser = new AppIdentityUser
        {
            Email = user.Email,
            UserName = user.Name,
            CreatedAt = user.CreatedAt,
        };

        var result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        
        await _signInManager.SignInAsync(newUser, false);
        
        return newUser.Id;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        var appUser = await _userManager.Users.FirstAsync(u => u.Id == userId);
        
        return new User(appUser.Id, appUser.Email, appUser.UserName);
    }
}
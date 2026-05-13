using System.Security.Claims;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

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
}
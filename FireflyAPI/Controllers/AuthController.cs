using FireflyAPI.Application.Interfaces;
using FireflyAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FireflyAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly SignInManager<AppIdentityUser> _signInManager;

    public AuthController(IAuthService authService, SignInManager<AppIdentityUser> signInManager)
    {
        _authService = authService;
        _signInManager = signInManager;
    }
    [HttpGet("external/google")]
    public IActionResult GoogleLogin()
    {
        string? redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

        return Challenge(properties, "Google");
    }

    [HttpGet("external/google/callback")]
    public async Task<IActionResult> GoogleResponse()
    {
        try
        {
            var user = await _authService.AuthenticateWithGoogleAsync();

            if (user == null)
                return BadRequest("Google login failed");

            return Redirect("http://localhost:5173");
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = "Google authentication failed",
                details = ex.Message
            });
        }
    }

}


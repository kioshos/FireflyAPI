using System.Security.Claims;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using FireflyAPI.Dtos;
using FireflyAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
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

            return Redirect("http://localhost:5173/main");
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto user)
    {
        var userId = await _authService.LoginAsync(user.Email, user.Password);

        if (userId == null)
            return Unauthorized("Invalid email or password");
        
        return Ok(userId);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new {message = "Logout successful"});
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        try
        {
            User user = new User(registerUserDto.Email, registerUserDto.Name);
            
            var userId = await _authService.RegisterAsync(user,registerUserDto.Password);
            
            return CreatedAtAction(nameof(Register),new {id = userId});
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
            return Unauthorized();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.CreatedAt
        });
    }

}


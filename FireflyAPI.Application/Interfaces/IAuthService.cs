namespace FireflyAPI.Application.Interfaces;

public interface IAuthService
{
    Task<Guid?> AuthenticateWithGoogleAsync();
}
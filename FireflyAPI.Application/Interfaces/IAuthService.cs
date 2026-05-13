using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IAuthService
{
    Task<Guid?> AuthenticateWithGoogleAsync();
    Task<Guid?> LoginAsync(string email, string password);
    Task<Guid?> RegisterAsync(User user, string password);
    Task LogoutAsync ();
    Task<User> GetUserByIdAsync(Guid userId);
}
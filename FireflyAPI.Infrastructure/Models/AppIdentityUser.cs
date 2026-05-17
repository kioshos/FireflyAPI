using Microsoft.AspNetCore.Identity;

namespace FireflyAPI.Infrastructure.Models;

public class AppIdentityUser : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }
}
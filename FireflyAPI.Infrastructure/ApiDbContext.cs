using FireflyAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure;

public class ApiDbContext : IdentityDbContext<AppIdentityUser, IdentityRole<Guid>,Guid>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options):
        base(options)
    {
        
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FireflyAPI.Infrastructure;

public class AppContextFactory : IDesignTimeDbContextFactory<ApiDbContext>
{
    public ApiDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
        optionsBuilder.UseSqlite("Data Source=Firefly.db");

        return new ApiDbContext(optionsBuilder.Options);
    }
}
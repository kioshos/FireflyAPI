using FireflyAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FireflyAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppIdentityUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApiDbContext>();
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "AuthCookie";

            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            options.ExpireTimeSpan = TimeSpan.FromDays(3);

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];

                options.CallbackPath = "/signin-google";

                options.CorrelationCookie.SameSite = SameSiteMode.None;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                options.SaveTokens = true;
                options.AccessType = "offline";

                options.ClaimActions.MapJsonKey("image", "picture");
                
            });
        
        
        
        return services;
    }
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        var efSettings = configuration.GetSection("EfCore")
            .Get<EfCoreSettings>();
        
        services.AddDbContext<ApiDbContext>((sp, options) =>
        {
            
            options
                .EnableSensitiveDataLogging(efSettings.EnableSensitiveDataLogging)
                .EnableDetailedErrors(efSettings.EnableDetailedErrors)
                .UseSqlite(connectionString, sqlite =>
                {
                    sqlite.CommandTimeout(efSettings.CommandTimeout);
                });
        });

        return services;
    }
}
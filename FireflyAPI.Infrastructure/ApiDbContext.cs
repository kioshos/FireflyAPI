using FireflyAPI.Domain.Entities;
using FireflyAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure;

public class ApiDbContext : IdentityDbContext<AppIdentityUser, IdentityRole<Guid>,Guid>
{
    public DbSet<Project> Projects =>  Set<Project>();
    public DbSet<Resource> Resources =>  Set<Resource>();
    public DbSet<Activity> Activities =>  Set<Activity>();
    public DbSet<ResourceRequirement> ResourceRequirements =>  Set<ResourceRequirement>();
    public DbSet<ActivityDependency> ActivityDependencies =>  Set<ActivityDependency>();
    public ApiDbContext(DbContextOptions<ApiDbContext> options):
        base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
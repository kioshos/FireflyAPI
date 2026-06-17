using FireflyAPI.Application.Interfaces;
using FireflyAPI.Application.Optimization;
using FireflyAPI.Application.Optimization.Interfaces;
using FireflyAPI.Application.Services;
using FireflyAPI.Domain.Entities;
using FireflyAPI.Infrastructure;
using FireflyAPI.Infrastructure.Repository;
using FireflyAPI.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRepository<Project>, ProjectRepository>();
builder.Services.AddScoped<IRepository<Activity>, ActivityRepository>();
builder.Services.AddScoped<IRepository<Resource>, ResourceRepository>();
builder.Services.AddScoped<IResourceRequirementRepository, ResourceRequirementRepository>();
builder.Services.AddScoped<IActivityDependencyRepository, ActivityDependencyRepository>();
builder.Services.AddScoped<IProjectOptimizer, PsoProjectOptimizer>();
builder.Services.AddScoped<IProjectOptimizationService, ProjectOptimizationService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<ActivityService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure.Repository;

public class ProjectRepository : IProjectRepository
{
    private readonly ApiDbContext _dbContext;

    public ProjectRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Project entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Projects.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project entity, CancellationToken cancellationToken = default)
    {
        var targetEntity = await _dbContext.Projects
            .FirstAsync(p => p.Id == entity.Id, cancellationToken);
        
        targetEntity.Name = entity.Name;
        targetEntity.Description = entity.Description;
        targetEntity.TotalRiskLevel = entity.TotalRiskLevel;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Project entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Projects.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects.Where(p => p.OwnerId == userId).ToListAsync(cancellationToken);
    }
}
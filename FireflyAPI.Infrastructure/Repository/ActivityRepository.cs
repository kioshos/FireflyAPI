using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure.Repository;

public class ActivityRepository : IActivityRepository
{
    private readonly ApiDbContext _dbContext;

    public ActivityRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Activity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Activities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Activity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Activities.AsNoTracking()
            .FirstOrDefaultAsync(ta => ta.Id == id, cancellationToken);
    }

    public async Task AddAsync(Activity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Activities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Activity entity, CancellationToken cancellationToken = default)
    {
        var targetEntity = await _dbContext.Activities
            .FirstAsync(ta => ta.Id == entity.Id, cancellationToken);

        targetEntity.Name = entity.Name;
        targetEntity.Duration = entity.Duration;
        targetEntity.RiskLevel = entity.RiskLevel;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Activity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Activities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Activity>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
       return await _dbContext.Activities
           .Where(a => a.ProjectId == projectId)
           .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Activity>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Activities
            .AsNoTracking()
            .Where(a => ids.Contains(a.Id))
            .ToListAsync(cancellationToken);

    }
}
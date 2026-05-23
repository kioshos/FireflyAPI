using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure.Repository;

public class ActivityDependencyRepository : IActivityDependencyRepository
{
    private readonly ApiDbContext _dbContext;

    public ActivityDependencyRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<ActivityDependency>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ActivityDependencies
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<ActivityDependency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task AddAsync(ActivityDependency entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.ActivityDependencies.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ActivityDependency entity, CancellationToken cancellationToken = default)
    {
       _dbContext.ActivityDependencies.Update(entity);
       await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ActivityDependency entity, CancellationToken cancellationToken = default)
    {
        _dbContext.ActivityDependencies.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ActivityDependency?> GetByCompositeKeysAsync(Guid taskId, Guid predecessorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ActivityDependencies
            .AsNoTracking()
            .FirstOrDefaultAsync(
                td => td.ActivityId == taskId &&
                      td.PredecessorActivityId == predecessorId, cancellationToken);
    }

    public async Task<IEnumerable<ActivityDependency>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ActivityDependencies.Where(ad => ad.ActivityId== activityId).ToListAsync(cancellationToken);
    }
}
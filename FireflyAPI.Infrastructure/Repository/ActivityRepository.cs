using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure.Repository;

public class ActivityRepository : IRepository<Activity>
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

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Activity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Activities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
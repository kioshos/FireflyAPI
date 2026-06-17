using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure.Repository;

public class ResourceRequirementRepository : IResourceRequirementRepository
{
    private readonly ApiDbContext _dbContext;

    public ResourceRequirementRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<ResourceRequirement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ResourceRequirements
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<ResourceRequirement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task AddAsync(ResourceRequirement entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.ResourceRequirements.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ResourceRequirement entity, CancellationToken cancellationToken = default)
    {
        _dbContext.ResourceRequirements.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(ResourceRequirement entity, CancellationToken cancellationToken = default)
    {
        _dbContext.ResourceRequirements.Remove(entity);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ResourceRequirement?> GetByCompositeKeysAsync(Guid taskId, Guid resourceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ResourceRequirements
            .AsNoTracking()
            .FirstOrDefaultAsync(
                td => td.ActivityId == taskId &&
                      td.ResourceId == resourceId, cancellationToken);
    }

    public async Task<IEnumerable<ResourceRequirement>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ResourceRequirements
            .AsNoTracking()
            .Where(rr => rr.ActivityId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ResourceRequirement>> GetByTaskIdsAsync(List<Guid> taskIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ResourceRequirements
            .AsNoTracking()
            .Where(r => taskIds.Contains(r.ActivityId))
            .ToListAsync(cancellationToken);
    }
}
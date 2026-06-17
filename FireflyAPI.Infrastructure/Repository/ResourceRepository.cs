using FireflyAPI.Application.Interfaces;
using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireflyAPI.Infrastructure.Repository;

public class ResourceRepository : IResourceRepository
{
    private readonly ApiDbContext _dbContext;

    public ResourceRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Resource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Resources
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Resource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Resources.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AddAsync(Resource entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Resources.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Resource entity, CancellationToken cancellationToken = default)
    {
        var targetEntity = await _dbContext.Resources
            .FirstAsync(r => r.Id == entity.Id, cancellationToken);

        targetEntity.Name = entity.Name;
        targetEntity.Amount = entity.Amount;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Resource entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Resources.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Resource>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _dbContext.Resources.Where(r =>r.ProjectId == projectId).ToListAsync(cancellationToken);
    }
    public async Task<List<Resource>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default)
    {
        return await _dbContext.Resources
            .AsNoTracking()
            .Where(r => ids.Contains(r.Id))
            .ToListAsync(ct);
    }

}
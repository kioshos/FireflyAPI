using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IActivityRepository : IRepository<Activity>
{
    public Task<IEnumerable<Activity>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Activity>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
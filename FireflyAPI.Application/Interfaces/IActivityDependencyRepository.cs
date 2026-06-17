using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IActivityDependencyRepository : IRepository<ActivityDependency>
{
    Task<ActivityDependency?> GetByCompositeKeysAsync(Guid taskId, Guid predecessorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActivityDependency>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActivityDependency>> GetByActivityIdsAsync(List<Guid> activityIds, CancellationToken cancellationToken = default);
    Task DeleteByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
}
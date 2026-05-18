using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IActivityDependencyRepository : IRepository<ActivityDependency>
{
    Task<ActivityDependency?> GetByCompositeKeysAsync(Guid taskId, Guid predecessorId, CancellationToken cancellationToken = default);
}
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IResourceRequirementRepository : IRepository<ResourceRequirement>
{
    Task<ResourceRequirement?> GetByCompositeKeysAsync(Guid taskId, Guid resourceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResourceRequirement>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResourceRequirement>> GetByTaskIdsAsync(List<Guid> taskIds, CancellationToken cancellationToken = default);
}
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IResourceRepository : IRepository<Resource>
{
    Task<IEnumerable<Resource>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<Resource>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default);
}
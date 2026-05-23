using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IResourceRepository : IRepository<Resource>
{
    public Task<IEnumerable<Resource>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);
}
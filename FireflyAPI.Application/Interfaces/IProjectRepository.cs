using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    public Task<IEnumerable<Project>> GetProjectsByUserId(Guid userId, CancellationToken cancellationToken);
}
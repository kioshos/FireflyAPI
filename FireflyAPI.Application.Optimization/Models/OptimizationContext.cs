using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization.Models;

public class OptimizationContext
{
    public required List<TaskActivity> Activities { get; init; }

    public required Dictionary<TaskActivity, PathInfo> Graph { get; init; }

    public required IReadOnlyCollection<Resource> Resources { get; init; }
}
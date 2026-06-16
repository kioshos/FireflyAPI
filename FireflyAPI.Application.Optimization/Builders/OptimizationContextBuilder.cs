using FireflyAPI.Application.Optimization.Models;
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization.Builders;

public class OptimizationContextBuilder
{
    public OptimizationContext Build(
        IEnumerable<Activity> activities,
        IEnumerable<Resource> resources,
        IEnumerable<ActivityDependency> dependencies,
        IEnumerable<Domain.Entities.ResourceRequirement> requirements)
    {
        var taskMap = new Dictionary<Guid, TaskActivity>();

        foreach (var activity in activities)
        {
            taskMap[activity.Id] = new TaskActivity
            {
                OriginalId = activity.Id,
                Name = activity.Name,
                Duration = new FuzzyDuration(activity.Duration, 0.5)
            };
        }

        foreach (var dependency in dependencies)
        {
            var activity = taskMap[dependency.ActivityId];
            var predecessor = taskMap[dependency.PredecessorActivityId];

            activity.Predecessors =
                activity.Predecessors.Append(predecessor).ToList();
        }

        foreach (var requirement in requirements)
        {
            var task = taskMap[requirement.ActivityId];

            var resource =
                resources.First(r => r.Id == requirement.ResourceId);

            task.ResourceRequirements.Add(
                new Optimization.Models.ResourceRequirement
                {
                    Resource = resource,
                    Amount = requirement.Amount
                });
        }

        var taskList = taskMap.Values.ToList();

        var graph =
            GraphBuilder.BuildAdjacencyList(taskList);

        return new OptimizationContext
        {
            Activities = taskList,
            Graph = graph,
            Resources = resources.ToList()
        };
    }
}
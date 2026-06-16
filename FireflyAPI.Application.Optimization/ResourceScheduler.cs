using FireflyAPI.Application.Optimization.Models;
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization;

public class ResourceScheduler
{
    private readonly Dictionary<TaskActivity, PathInfo> _graph;
    private readonly ResourceManager _resourceManager;

    public ResourceScheduler(Dictionary<TaskActivity, PathInfo> graph, IEnumerable<Resource> resources)
    {
        _graph = graph;
        _resourceManager = new ResourceManager(resources);
    }

    private int GetDependencyReady(TaskActivity activity, List<ScheduledTask> scheduledTasks)
    {
        if (!_graph[activity].Predecessors.Any())
        {
            return 0;
        }
        return scheduledTasks
            .Where(sT => _graph[activity].Predecessors
                .Contains(sT.Activity))
            .Max(sT => sT.Finish);
    }

    public List<ScheduledTask> Build(List<TaskActivity> orderedTasks)
    {
        var result = new List<ScheduledTask>();

        foreach (var task in orderedTasks)
        {
            int dependencyReady = GetDependencyReady(task, result);
            int actualStart = _resourceManager.FindEarliestSlot(task,dependencyReady);
            int actualFinish = (int)(actualStart + task.Duration.Defuzzification());
            
            _resourceManager.Reserve(task, actualStart);
            result.Add(new ScheduledTask()
            {
                Activity = task,
                Start = actualStart,
                Finish = actualFinish
            });
        }
        
        return result;
    }
}
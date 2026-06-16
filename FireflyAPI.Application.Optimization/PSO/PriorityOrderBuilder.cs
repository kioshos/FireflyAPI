using FireflyAPI.Application.Optimization.Models;

namespace FireflyAPI.Application.Optimization.PSO;

public class PriorityOrderBuilder
{
    public List<TaskActivity> Build(IReadOnlyList<TaskActivity> tasks, Particle particle)
    {
        var priorities = tasks.Select((task, index) => new
            {
                Task = task,
                Priority = particle.Position[index]
            })
            .ToDictionary(t => t.Task, t => t.Priority);

        var result = new List<TaskActivity>();

        var remaining = new HashSet<TaskActivity>(tasks);

        while (remaining.Count > 0)
        {
            var available = remaining
                .Where(t => t.Predecessors
                    .All(p => result
                        .Contains(p)))
                .OrderByDescending(t => priorities[t])
                .ToList();
            
            var chosen = available.First();
            result.Add(chosen);
            remaining.Remove(chosen);
        }
        
        return result;
    }
}
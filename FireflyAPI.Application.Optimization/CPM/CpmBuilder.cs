using FireflyAPI.Application.Optimization.Models;

namespace FireflyAPI.Application.Optimization.CPM;

public class CpmBuilder
{
    private Dictionary<TaskActivity, PathInfo> _graph;

    public CpmBuilder(Dictionary<TaskActivity, PathInfo> graph)
    {
        _graph = graph;
    }

    public void Build()
    {
        var sortedNodes = TopologicalSort();

        if (sortedNodes.Count != _graph.Count)
            throw new Exception("Graph has a cycle!");

        FillEarliestValues(sortedNodes);

        sortedNodes.Reverse();

        FillLatestValues(sortedNodes);
    }

    public List<TaskActivity> GetCriticalPath()
    {
        List<TaskActivity> result = new List<TaskActivity>();

        foreach (var activity in _graph.Keys)
        {
            if (_graph[activity].Slack == 0)
                result.Add(activity);
        }

        return result;
    }

    public List<TaskActivity> GetTopologicalOrder()
    {
        return TopologicalSort();
    }
    private List<TaskActivity> TopologicalSort()
    {
        var inDegree = new Dictionary<TaskActivity, int>();
        var result = new List<TaskActivity>();

        foreach (var node in _graph.Keys)
        {
            inDegree[node] = _graph[node].Predecessors.Count();
        }

        var queue = new Queue<TaskActivity>(inDegree.Count);

        foreach (var kvp in inDegree)
        {
            if (kvp.Value == 0)
                queue.Enqueue(kvp.Key);
        }

        while (queue.Any())
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var successor in _graph[current].Successors)
            {
                inDegree[successor] -= 1;

                if (inDegree[successor] == 0)
                    queue.Enqueue(successor);
            }
        }

        return result;
    }

    private void FillEarliestValues(List<TaskActivity> activities)
    {
        if (!_graph.Any())
            return;

        foreach (var activity in activities)
        {
            var info = _graph[activity];

            if (!info.Predecessors.Any())
            {
                info.EarliestStart = 0;
            }
            else
            {
                info.EarliestStart = info.Predecessors.Max(p => _graph[p].EarliestFinish);
            }

            info.EarliestFinish = info.EarliestStart + activity.Duration.Defuzzification();
        
        }
    }

    private void FillLatestValues(List<TaskActivity> reverseActivities)
    {
        if (!_graph.Any())
            return;

        double maxProjectDuration = _graph.Values.Max(info => info.EarliestFinish);

        foreach (var activity in reverseActivities)
        {
            var info = _graph[activity];

            if (!info.Successors.Any())
            {
                info.LatestFinish = maxProjectDuration;
            }
            else
            {
                info.LatestFinish = info.Successors.Min(s => _graph[s].LatestStart);
            }

            info.LatestStart = info.LatestFinish -  activity.Duration.Defuzzification();
        }
    }
}
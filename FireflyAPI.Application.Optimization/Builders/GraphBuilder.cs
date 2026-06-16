using FireflyAPI.Application.Optimization.Models;

namespace FireflyAPI.Application.Optimization.Builders;

public static class GraphBuilder
{
    public static Dictionary<TaskActivity, PathInfo> BuildAdjacencyList(List<TaskActivity> activities)
    {
        var successors = new Dictionary<TaskActivity, List<TaskActivity>>(activities.Count);
        
        foreach (var activity in activities)
        {
            successors[activity] = new List<TaskActivity>();
        }
        
        foreach (var activity in activities)
        {
            foreach (var predecessor in activity.Predecessors)
            {
                if (!successors.ContainsKey(predecessor))
                {
                    successors[predecessor] = new List<TaskActivity>();
                }
                
                successors[predecessor].Add(activity);
            }
        }
        
        var result = new Dictionary<TaskActivity, PathInfo>();
        
        foreach (var pair in activities)
        {
            var tmp = new PathInfo(pair.Predecessors, successors[pair]);
            result[pair] = tmp; 
        }
        
        return result;
    }
}
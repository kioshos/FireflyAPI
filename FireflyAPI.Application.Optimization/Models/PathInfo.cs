namespace FireflyAPI.Application.Optimization.Models;

public class PathInfo
{
    public double EarliestStart { get; set; }
    public double EarliestFinish { get; set; }
    public double LatestStart { get; set; }
    public double LatestFinish { get; set; }

    public double Slack
    {
        get
        {
            return LatestStart - EarliestStart;
        }
    }

    public IEnumerable<TaskActivity> Predecessors { get; private set; } 
    public IEnumerable<TaskActivity> Successors { get; private set; }

    public PathInfo(IEnumerable<TaskActivity> predecessors, IEnumerable<TaskActivity> successors)
    {
        Predecessors = predecessors;
        Successors = successors;
    }
}
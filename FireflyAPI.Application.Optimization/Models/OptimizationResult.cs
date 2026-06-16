namespace FireflyAPI.Application.Optimization.Models;

public class OptimizationResult
{
    public double Makespan { get; set; }

    public List<ScheduledTask> Schedule { get; set; } = new();
}
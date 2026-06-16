using FireflyAPI.Application.Optimization.Interfaces;
using FireflyAPI.Application.Optimization.Models;
using FireflyAPI.Application.Optimization.PSO;

namespace FireflyAPI.Application.Optimization;

public class PsoProjectOptimizer : IProjectOptimizer
{
    private readonly SwarmHandler _swarmHandler;

    public PsoProjectOptimizer()
    {
        _swarmHandler = new SwarmHandler();
    }

    public OptimizationResult Optimize(OptimizationContext context)
    {
        var bestParticle = _swarmHandler.RunOptimization(context, 50);

        var orderBuilder = new PriorityOrderBuilder();

        var orderedTasks =
            orderBuilder.Build(
                context.Activities,
                bestParticle);

        var scheduler =
            new ResourceScheduler(
                context.Graph,
                context.Resources);

        var schedule = scheduler.Build(orderedTasks);

        return new OptimizationResult
        {
            Makespan = bestParticle.BestFitness,
            Schedule = schedule
        };
    }
}
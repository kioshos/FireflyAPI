using FireflyAPI.Application.Optimization.Models;

namespace FireflyAPI.Application.Optimization.Interfaces;

public interface IProjectOptimizer
{
    OptimizationResult Optimize(OptimizationContext context);
}
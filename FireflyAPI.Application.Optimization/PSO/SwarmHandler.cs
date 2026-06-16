using FireflyAPI.Application.Optimization.Models;
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization.PSO;

public class SwarmHandler
{
    private List<Particle> InitializeSwarm(int swarmSize, List<TaskActivity> tasks, Dictionary<TaskActivity, PathInfo> graph)
    {
        var swarm = new List<Particle>();
        var random = new Random();
        int numTasks = tasks.Count;

        for (int i = 0; i < swarmSize; i++)
        {
            var particle = new Particle(numTasks);

            if (i == 0) 
            {
                // Для першої частинки даємо пріоритет на основі резерву часу (Slack)
                double maxSlack = graph.Values.Max(t => t.Slack);
                for (int j = 0; j < numTasks; j++)
                {
                    particle.Position[j] = 1.0 - (graph[tasks[j]].Slack / (maxSlack + 0.001));
                    particle.Velocity[j] = 0.0;
                }
            }
            else 
            {
                // Інші частинки отримують випадкові пріоритети
                for (int j = 0; j < numTasks; j++)
                {
                    particle.Position[j] = random.NextDouble(); 
                    particle.Velocity[j] = (random.NextDouble() * 2 - 1) * 0.1; 
                }
            }
            swarm.Add(particle);
        }
        return swarm;
    }

    public Particle RunOptimization(OptimizationContext context, int maxIterations)
    {
        int swarmSize = 30;
        double w = 0.7298;
        double c1 = 1.49618;
        double c2 = 1.49618;
        var random = new Random();
        
        var tasks = context.Activities;
        var graph = context.Graph;
        var resources = context.Resources.ToList();

        if (graph == null || graph.Count == 0)
            throw new InvalidOperationException("Optimization graph is empty. Check project data.");
        
        var swarm = InitializeSwarm(swarmSize, tasks, graph);
        double[] globalBestPosition = new double[tasks.Count];
        double globalBestFitness = double.MaxValue;

        for (int iter = 0; iter < maxIterations; iter++)
        {
            foreach (var particle in swarm)
            {
                // Рахуємо, наскільки хороший цей варіант розкладу
                particle.CurrentFitness = EvaluateFitness(particle.Position, tasks, graph, resources);

                if (particle.CurrentFitness < particle.BestFitness)
                {
                    particle.BestFitness = particle.CurrentFitness;
                    Array.Copy(particle.Position, particle.BestPosition, particle.Position.Length);
                }

                if (particle.CurrentFitness < globalBestFitness)
                {
                    globalBestFitness = particle.CurrentFitness;
                    Array.Copy(particle.Position, globalBestPosition, particle.Position.Length);
                }
            }

            foreach (var particle in swarm)
            {
                for (int j = 0; j < tasks.Count; j++)
                {
                    double r1 = random.NextDouble();
                    double r2 = random.NextDouble();

                    particle.Velocity[j] = w * particle.Velocity[j] + 
                                           c1 * r1 * (particle.BestPosition[j] - particle.Position[j]) + 
                                           c2 * r2 * (globalBestPosition[j] - particle.Position[j]);

                    particle.Position[j] += particle.Velocity[j];
                }
            }
        }

        return new Particle(tasks.Count) { Position = globalBestPosition, BestFitness = globalBestFitness };
    }

    private double EvaluateFitness(double[] particlePosition, List<TaskActivity> tasks, Dictionary<TaskActivity, PathInfo> graph, List<Resource> resources)
    {
        // 1. Створюємо частинку з поточними пріоритетами
        var particle = new Particle(tasks.Count);
        Array.Copy(particlePosition, particle.Position, particlePosition.Length);

        // 2. Будуємо чергу завдань на основі цих пріоритетів
        var orderBuilder = new PriorityOrderBuilder();
        var orderedTasks = orderBuilder.Build(tasks, particle);

        // 3. Складаємо розклад з урахуванням обмежених ресурсів
        var scheduler = new ResourceScheduler(graph, resources);
        var schedule = scheduler.Build(orderedTasks);

        // 4. Час завершення найостаннішого завдання - це і є наш результат (чим менше, тим краще)
        if (!schedule.Any()) return 0;
        
        return schedule.Max(s => s.Finish);
    }
}
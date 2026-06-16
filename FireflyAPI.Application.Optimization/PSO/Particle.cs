namespace FireflyAPI.Application.Optimization.PSO;

public class Particle
{
    public double[] Position { get; set; } 
    public double[] Velocity { get; set; } 
    
    public double[] BestPosition { get; set; } 
    public double BestFitness { get; set; } // Makespan for Pbest
    public double CurrentFitness { get; set; } 
    
    public Particle(int numberOfTasks)
    {
        Position = new double[numberOfTasks];
        Velocity = new double[numberOfTasks];
        BestPosition = new double[numberOfTasks];
        BestFitness = double.MaxValue; 
    }

    public override string ToString()
    {
        return string.Join(", ", Position);
    }
}
namespace FireflyAPI.Application.Optimization.Models;

public struct FuzzyDuration
{
    public int Optimistic {get; init;}
    public int Medium {get; init;}
    public int Pessimistic {get; init;}
   
    private double TaskRiskLevel {get; set;}

    
    public FuzzyDuration(int medium, double taskRiskLevel)
    {
        TaskRiskLevel = taskRiskLevel;
        Pessimistic = (int)(medium * 1.5);
        Medium = medium;
        Optimistic = (int)(medium * 0.8);
    }
    
    private FuzzyDuration(int optimistic, int medium,int pessimistic)
    {
        Optimistic = optimistic;
        Medium = medium;
        Pessimistic = pessimistic;
    }
    
    public static FuzzyDuration operator +(FuzzyDuration a, FuzzyDuration b) 
        => new FuzzyDuration(a.Pessimistic + b.Pessimistic, a.Medium+b.Medium, a.Optimistic+b.Optimistic);
    
    public static FuzzyDuration operator -(FuzzyDuration a, FuzzyDuration b) 
        => new FuzzyDuration(a.Pessimistic - b.Pessimistic, a.Medium-b.Medium, a.Optimistic-b.Optimistic);

    public override string ToString()
    {
        return $"{Pessimistic}-{Medium}-{Optimistic}";
    }

    public double Defuzzification() // this lambda in future could be a linguistic variable or smth like that lambda Є [0,1]
    {
        double leftCenter = (Optimistic + Medium) / 2.0;
        double rightCenter = (Medium + Pessimistic) / 2.0;
        
        double taskDuration = (1.0 - TaskRiskLevel) * leftCenter + TaskRiskLevel * rightCenter;
        
        return taskDuration;
    }
}
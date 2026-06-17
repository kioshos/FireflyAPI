namespace FireflyAPI.Domain.Entities;

public class Activity
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public int Duration { get; set; }
    public string Name { get; set; }
    public double RiskLevel { get; set; }
    private Activity(){}
    
    public Activity(Guid projectId, int duration, string name, double riskLevel)
    {
        Id = Guid.CreateVersion7();
        ProjectId = projectId;
        Duration = duration;
        Name = name;
        RiskLevel = riskLevel;
    }
}
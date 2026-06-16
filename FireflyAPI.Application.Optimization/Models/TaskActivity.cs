using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization.Models;

public class TaskActivity
{
    public Guid OriginalId { get; init; }
    public string Name { get; set; }
    public FuzzyDuration Duration { get; set; }
    
    public double FinalDuration { get; set; }

    public double RiskLevel { get; set; } = 0.5; // lambda
    public IEnumerable<TaskActivity> Predecessors { get; set; } 
    
    public List<ResourceRequirement> ResourceRequirements { get; set; }

    public TaskActivity(IEnumerable<TaskActivity> predecessors)
    {
        Predecessors = predecessors;             
    }

    public TaskActivity()
    {
        Predecessors = new List<TaskActivity>();
        ResourceRequirements = new List<ResourceRequirement>();
    }

    public void AssignResource(Resource resource, double amount)
    {
        ResourceRequirements.Add(new ResourceRequirement()
        {
            Resource = resource,
            Amount = amount
        });
    }
}
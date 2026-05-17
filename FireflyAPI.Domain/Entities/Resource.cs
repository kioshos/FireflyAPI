namespace FireflyAPI.Domain.Entities;

public class Resource
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public double Amount { get; set; }
    public string Name { get; set; }

    private Resource(){ }
    
    public Resource(Guid projectId, double amount, string name)
    {
        Id = Guid.CreateVersion7();
        ProjectId = projectId;
        Amount = amount;
        Name = name;
    }
}
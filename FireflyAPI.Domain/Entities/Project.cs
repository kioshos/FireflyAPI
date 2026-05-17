namespace FireflyAPI.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    private Project(){ }

    public Project(Guid ownerId, string name, string description)
    {
        Id = Guid.CreateVersion7();
        OwnerId = ownerId;
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }
}
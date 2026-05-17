namespace FireflyAPI.Domain.Entities;

public class ResourceRequirement
{
    public Guid ActivityId { get; set; }
    public Guid ResourceId { get; set; }
    public double Amount { get; set; }
    
    private ResourceRequirement(){ }

    public ResourceRequirement(Guid activityId, Guid resourceId, double amount)
    {
        ActivityId = activityId;
        ResourceId = resourceId;
        Amount = amount;
    }
}
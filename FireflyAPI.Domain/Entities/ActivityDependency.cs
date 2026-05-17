namespace FireflyAPI.Domain.Entities;

public class ActivityDependency
{
    public Guid ActivityId { get; set; }
    public Guid PredecessorActivityId { get; set; }
    
    private ActivityDependency(){ }

    public ActivityDependency(Guid activityId, Guid predecessorActivityId)
    {
        ActivityId = activityId;
        PredecessorActivityId = predecessorActivityId;
    }
}
namespace FireflyAPI.Application.Optimization.Models;

public class ScheduledTask
{
    public TaskActivity Activity { get; set; }
    public int Start { get; set; }
    public int Finish { get; set; }
}
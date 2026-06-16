namespace FireflyAPI.Application.Dtos;

public class OptimizedTaskDto
{
    public Guid ActivityId { get; set; }

    public string Name { get; set; }

    public int Start { get; set; }

    public int Finish { get; set; }
}
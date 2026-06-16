namespace FireflyAPI.Application.Dtos;

public class OptimizationResultDto
{
    public double Makespan { get; set; }

    public List<OptimizedTaskDto> Tasks { get; set; }
}
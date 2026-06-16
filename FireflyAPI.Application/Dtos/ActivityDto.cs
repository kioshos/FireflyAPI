namespace FireflyAPI.Application.Dtos;

public sealed class ActivityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Duration { get; set; }
}
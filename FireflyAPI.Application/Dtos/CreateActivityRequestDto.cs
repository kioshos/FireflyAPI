namespace FireflyAPI.Application.Dtos;

public sealed class CreateActivityRequestDto
{
    public string Name { get; init; }
    public int Duration { get; init; }
}
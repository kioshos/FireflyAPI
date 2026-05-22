namespace FireflyAPI.Application.Dtos;

public sealed class CreateProjectRequestDto
{
    public required string Name { get; init; }
    public string Description { get; init; }
}
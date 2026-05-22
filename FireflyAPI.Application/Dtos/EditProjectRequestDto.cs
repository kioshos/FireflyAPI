namespace FireflyAPI.Application.Dtos;

public sealed class EditProjectRequestDto
{
    public required string Name { get; init; }
    public string Description { get; init; }
}
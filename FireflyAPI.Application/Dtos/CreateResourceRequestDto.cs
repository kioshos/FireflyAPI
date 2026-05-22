namespace FireflyAPI.Application.Dtos;

public sealed class CreateResourceRequestDto
{
    public double Amount { get; init; }
    public string Name { get; init; }
}
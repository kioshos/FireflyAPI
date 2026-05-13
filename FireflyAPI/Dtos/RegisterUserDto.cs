namespace FireflyAPI.Dtos;

public sealed class RegisterUserDto
{
    public string Email { get; init; }
    public string Name { get; init; }
    public string Password { get; init; }
}
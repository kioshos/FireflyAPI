namespace FireflyAPI.Application.Interfaces;

public interface ICurrentUserService
{
    public Guid? UserId { get; }
}
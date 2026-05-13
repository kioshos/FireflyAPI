namespace FireflyAPI.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }

    public User(string email, string name)
    {
        Id = Guid.CreateVersion7();
        Email = email;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
    public User(Guid id, string email, string name)
    {
        Id = id;
        Email = email;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
}
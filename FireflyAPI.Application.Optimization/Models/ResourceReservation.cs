using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization.Models;

public class ResourceReservation
{
    public Resource Resource { get; set; }
    public TaskActivity TaskActivity { get; set; }  
    public double Amount { get; set; }
    public int Start { get; set; }
    public int Finish { get; set; }
}
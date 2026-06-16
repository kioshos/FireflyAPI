using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization.Models;

public class ResourceRequirement
{
    public Resource Resource { get; set; }
    public double Amount { get; set; }
}
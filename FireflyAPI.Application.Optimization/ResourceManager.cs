using FireflyAPI.Application.Optimization.Models;
using FireflyAPI.Domain.Entities;

namespace FireflyAPI.Application.Optimization;

public class ResourceManager
{
    private readonly Dictionary<Resource, List<ResourceReservation>> _reservations = new();

    public ResourceManager(IEnumerable<Resource> resources)
    {
        foreach (var resource in resources)
        {
            _reservations[resource] = new List<ResourceReservation>();
        }
    }

    public void Reserve(TaskActivity activity, int start)
    {
        int finish = (int)(start + activity.Duration.Defuzzification());

        foreach (var resource in activity.ResourceRequirements)
        {
            _reservations[resource.Resource].Add(new ResourceReservation()
            {
                Resource = resource.Resource,
                TaskActivity = activity,
                Start = start,
                Finish = finish,
                Amount = resource.Amount
            });
        }
    }

    public int FindEarliestSlot(TaskActivity activity, int earliestStart)
    {
        int candidate = earliestStart;

        while(true)
        {
            if (IsAvailable(activity, candidate))
            {
                return candidate;
            }
            //todo: математично порахувати після спання
            candidate++;
        }
    }
        
    private bool IsAvailable(TaskActivity activity, int start)
    {
        int finish = (int)(start + activity.Duration.Defuzzification());

        foreach (var resource in activity.ResourceRequirements)
        {
            var reservations = _reservations[resource.Resource];

            double used = 0.0;

            foreach (var reservation in reservations)
            {
                bool overlap = start < reservation.Finish && finish > reservation.Start;

                if (overlap)
                {
                    used += reservation.Amount;
                }
            }

            if (used + resource.Amount > resource.Resource.Amount)
            {
                return false;
            }
        }
        
        return true;
    }
}
using MiniHW_1.Zoo.Domain.Abstractions;

namespace MiniHW_1.Zoo.Domain.Entities.Firms;

public class VeterinaryClinic
{
    public HealthStatus CheckHealth(Animal animal)
    {
        int healthCheck = 1; // 0 - Sick, 1 - Healthy
        if (animal.Food > 100 || animal.Food <= 0)
        {
            healthCheck = 0; // Animal has problems with native
        }

        animal.HealthStatus = healthCheck switch
        {
            0 => HealthStatus.Sick,
            1 => HealthStatus.Healthy,
            _ => HealthStatus.NeedsCheckup
        };

        return animal.HealthStatus;
    }
}
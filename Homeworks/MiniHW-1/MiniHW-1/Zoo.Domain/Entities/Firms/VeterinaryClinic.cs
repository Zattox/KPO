﻿using MiniHW_1.Zoo.Domain.Entities.Creatures;

namespace MiniHW_1.Zoo.Domain.Entities.Firms;

public class VeterinaryClinic
{
    public HealthStatus CheckHealth(Animal animal)
    {
        Random random = new Random();
        int healthCheck = random.Next(2); // 0 - Sick, 1 - Healthy
        if (animal.Food > 100 || animal.Food == 0)
        {
            healthCheck = 0; // У животного проблемы с питанием.
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
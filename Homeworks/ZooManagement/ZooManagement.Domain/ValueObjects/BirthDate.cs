using System;

namespace ZooManagement.Domain.ValueObjects
{
    // Represents the birth date of an animal
    public class BirthDate
    {
        public DateTime Value { get; }

        public BirthDate(DateTime value)
        {
            if (value > DateTime.UtcNow)
                throw new ArgumentException("Birth date cannot be in the future.");
            Value = value;
        }

        // Calculates the age of the animal
        public int CalculateAge() => DateTime.UtcNow.Year - Value.Year;
    }
}
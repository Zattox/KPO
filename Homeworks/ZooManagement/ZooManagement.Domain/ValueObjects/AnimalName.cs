using System;

namespace ZooManagement.Domain.ValueObjects
{
    // Represents the name of an animal
    public class AnimalName
    {
        public string Value { get; }

        public AnimalName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Animal name cannot be empty.");
            if (value.Length > 50)
                throw new ArgumentException("Animal name cannot exceed 50 characters.");
            Value = value;
        }

        public override bool Equals(object obj) => obj is AnimalName name && Value == name.Value;
        public override int GetHashCode() => Value.GetHashCode();
        public static implicit operator string(AnimalName name) => name.Value;
    }
}
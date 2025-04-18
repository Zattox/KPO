using System;

namespace ZooManagement.Domain.ValueObjects
{
    // Represents the maximum capacity of an enclosure
    public class EnclosureCapacity
    {
        public int Value { get; }

        public EnclosureCapacity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Max capacity must be positive.");
            Value = value;
        }
    }
}
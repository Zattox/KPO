using System;

namespace ZooManagement.Domain.ValueObjects
{
    // Represents the size of an enclosure
    public class EnclosureSize
    {
        public int Value { get; }

        public EnclosureSize(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Size must be positive.");
            Value = value;
        }
    }
}
using System;
using ZooManagement.Domain.Enums;

namespace ZooManagement.Application.DTOs
{
    // Data transfer object for Enclosure
    public class EnclosureDto
    {
        public Guid Id { get; set; }
        public EnclosureType Type { get; set; }
        public int Size { get; set; }
        public int CurrentAnimalCount { get; set; }
        public int MaxCapacity { get; set; }
    }
}
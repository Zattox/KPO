using System;
using System.Linq;
using ZooManagement.Application.Abstractions;

namespace ZooManagement.Application.Services
{
    // Provides statistics about the zoo
    public class ZooStatisticsService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;

        public ZooStatisticsService(IAnimalRepository animalRepository, IEnclosureRepository enclosureRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        // Gets the total number of animals
        public int GetTotalAnimals()
        {
            return _animalRepository.GetAll().Count();
        }

        // Gets the number of free enclosures
        public int GetFreeEnclosures()
        {
            return _enclosureRepository.GetAll().Count(e => e.CurrentAnimalCount < e.MaxCapacity);
        }
    }
}
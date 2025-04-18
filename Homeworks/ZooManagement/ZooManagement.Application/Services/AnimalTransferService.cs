using System;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Events;

namespace ZooManagement.Application.Services
{
    // Handles animal transfer between enclosures
    public class AnimalTransferService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;

        public AnimalTransferService(IAnimalRepository animalRepository, IEnclosureRepository enclosureRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        // Transfers an animal to a new enclosure
        public AnimalMovedEvent TransferAnimal(Guid animalId, Guid newEnclosureId)
        {
            Animal animal = _animalRepository.GetById(animalId);
            if (animal == null)
                throw new InvalidOperationException("Animal not found.");

            Enclosure newEnclosure = _enclosureRepository.GetById(newEnclosureId);
            if (newEnclosure == null)
                throw new InvalidOperationException("Enclosure not found.");

            // Remove from old enclosure if applicable
            if (animal.EnclosureId.HasValue)
            {
                Enclosure oldEnclosure = _enclosureRepository.GetById(animal.EnclosureId.Value);
                if (oldEnclosure != null)
                {
                    oldEnclosure.RemoveAnimal(animal);
                    _enclosureRepository.Update(oldEnclosure);
                }
            }

            newEnclosure.AddAnimal(animal);
            var movedEvent = animal.MoveToEnclosure(newEnclosure);

            _animalRepository.Update(animal);
            _enclosureRepository.Update(newEnclosure);

            return movedEvent;
        }
    }
}
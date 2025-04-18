using System;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Events;

namespace ZooManagement.Application.Services
{
    public class FeedingOrganizationService
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;
        private readonly IAnimalRepository _animalRepository;

        public FeedingOrganizationService(IFeedingScheduleRepository feedingScheduleRepository, IAnimalRepository animalRepository)
        {
            _feedingScheduleRepository = feedingScheduleRepository ?? throw new ArgumentNullException(nameof(feedingScheduleRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
        }

        public FeedingTimeEvent CompleteFeeding(Guid scheduleId)
        {
            var schedule = _feedingScheduleRepository.GetById(scheduleId);
            if (schedule == null)
                throw new InvalidOperationException("Feeding schedule not found.");

            var animal = _animalRepository.GetById(schedule.AnimalId);
            if (animal == null)
                throw new InvalidOperationException("Animal not found.");

            if (animal.FavoriteFood != schedule.FoodType)
                throw new InvalidOperationException("Food type does not match animal's favorite food.");

            var @event = schedule.MarkCompleted();
            _feedingScheduleRepository.Update(schedule);
            return @event;
        }
    }
}
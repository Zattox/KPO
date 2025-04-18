using System;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Events;

namespace ZooManagement.Application.Services
{
    // Manages feeding schedules and feeding process
    public class FeedingOrganizationService
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;
        private readonly IAnimalRepository _animalRepository;

        public FeedingOrganizationService(IFeedingScheduleRepository feedingScheduleRepository, IAnimalRepository animalRepository)
        {
            _feedingScheduleRepository = feedingScheduleRepository ?? throw new ArgumentNullException(nameof(feedingScheduleRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
        }

        // Marks a feeding as completed
        public FeedingTimeEvent CompleteFeeding(Guid scheduleId)
        {
            var schedule = _feedingScheduleRepository.GetById(scheduleId);
            if (schedule == null)
                throw new InvalidOperationException("Feeding schedule not found.");

            var animal = _animalRepository.GetById(schedule.AnimalId);
            if (animal == null)
                throw new InvalidOperationException("Animal not found.");

            animal.Feed(schedule.FoodType);
            var timeEvent = schedule.MarkCompleted();

            _feedingScheduleRepository.Update(schedule);

            return timeEvent;
        }
    }
}
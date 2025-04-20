using System;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.Events;
using ZooManagement.Domain.ValueObjects;

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
        public void AddFeedingSchedule(Guid animalId, FeedingTime feedingTime, FoodType foodType)
        {
            var animal = _animalRepository.GetById(animalId);
            if (animal == null)
                throw new InvalidOperationException("Animal not found.");

            var schedule = new FeedingSchedule(animalId, feedingTime, foodType);
            schedule.Update(animal, feedingTime, foodType);
            _feedingScheduleRepository.Add(schedule);
        }

        public FeedingTimeEvent CompleteFeeding(Guid scheduleId)
        {
            var schedule = _feedingScheduleRepository.GetById(scheduleId);
            if (schedule == null)
                throw new InvalidOperationException("Feeding schedule not found.");

            var animal = _animalRepository.GetById(schedule.AnimalId);
            if (animal == null)
                throw new InvalidOperationException("Animal not found.");

            animal.Feed(schedule.FoodType);
            var @event = schedule.MarkCompleted();
            _feedingScheduleRepository.Update(schedule);
            return @event;
        }
        public void UpdateFeedingSchedule(Guid scheduleId, FeedingTime feedingTime, FoodType foodType)
        {
            var schedule = _feedingScheduleRepository.GetById(scheduleId);
            if (schedule == null)
                throw new InvalidOperationException("Feeding schedule not found.");

            var animal = _animalRepository.GetById(schedule.AnimalId);
            if (animal == null)
                throw new InvalidOperationException("Animal not found.");

            schedule.Update(animal, feedingTime, foodType);
            _feedingScheduleRepository.Update(schedule);
        }
    }
}